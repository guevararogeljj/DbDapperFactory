using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using MySqlConnector;
using System.Data.SQLite;

namespace DbDapperFactory;

internal sealed class DapperConnectionFactory : IDapperConnectionFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyDictionary<string, INamedDbConnectionFactoryRegistration> _registrations;
    private readonly ConcurrentDictionary<string, (DatabaseProvider Provider, string ConnectionString)> _configs = new(StringComparer.OrdinalIgnoreCase);

    public DapperConnectionFactory(
        IServiceProvider serviceProvider,
        IEnumerable<INamedDbConnectionFactoryRegistration> registrations)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        if (registrations is null)
        {
            throw new ArgumentNullException(nameof(registrations));
        }

        var map = new Dictionary<string, INamedDbConnectionFactoryRegistration>(StringComparer.OrdinalIgnoreCase);
        var duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var registration in registrations)
        {
            if (!map.TryAdd(registration.Name, registration))
            {
                duplicates.Add(registration.Name);
            }
        }

        if (duplicates.Count > 0)
        {
            throw new InvalidOperationException(
                $"Duplicate Dapper connection registrations: {string.Join(", ", duplicates.OrderBy(x => x))}.");
        }

        _registrations = map;
    }

    internal DapperConnectionFactory(
        IConfiguration configuration,
        IEnumerable<(string Name, DatabaseProvider Provider)> databases)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(databases);

        _serviceProvider = EmptyServiceProvider.Instance;
        _registrations = new Dictionary<string, INamedDbConnectionFactoryRegistration>(StringComparer.OrdinalIgnoreCase);

        foreach (var (name, provider) in databases.Distinct())
        {
            var cs = configuration.GetConnectionString(name)
                     ?? throw new InvalidOperationException($"Connection string '{name}' was not found.");

            _configs[name] = (provider, cs);
        }
    }

    public DbConnection CreateConnection(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Connection name is required.", nameof(name));
        }

        if (_registrations.TryGetValue(name, out var registration))
        {
            return registration.Create(_serviceProvider);
        }

        if (_configs.TryGetValue(name, out var cfg))
        {
            return cfg.Provider switch
            {
                DatabaseProvider.SqlServer => new SqlConnection(cfg.ConnectionString),
                DatabaseProvider.Postgres => new NpgsqlConnection(cfg.ConnectionString),
                DatabaseProvider.Oracle => new OracleConnection(cfg.ConnectionString),
                DatabaseProvider.MySql => new MySqlConnection(cfg.ConnectionString),
                DatabaseProvider.Sqlite => new SQLiteConnection(cfg.ConnectionString),
                _ => throw new NotSupportedException($"Provider '{cfg.Provider}' is not supported.")
            };
        }

        var available = _registrations.Keys.OrderBy(x => x).ToArray();
        if (_configs.Count > 0)
        {
            available = available.Concat(_configs.Keys).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray();
        }

        var availableText = available.Length == 0 ? "<none>" : string.Join(", ", available);
        throw new InvalidOperationException($"No configuration registered for '{name}'. Available: {availableText}.");
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public static readonly EmptyServiceProvider Instance = new();

        public object? GetService(Type serviceType) => null;
    }
}
