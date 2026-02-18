using System.Data;

namespace DbDapperFactory.Core;

/// <summary>
/// Base implementation of IDbConnectionFactory that manages named connections.
/// </summary>
public abstract class DbConnectionFactoryBase : IDbConnectionFactory
{
    private readonly DbConnectionFactoryOptions _options;
    private readonly string? _defaultConnectionName;

    protected DbConnectionFactoryBase(DbConnectionFactoryOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _defaultConnectionName = _options.DefaultConnection?.Name;
    }

    public IDbConnection CreateConnection(string? connectionName = null)
    {
        var config = GetConnectionConfig(connectionName);
        return CreateConnectionCore(config);
    }

    public string GetConnectionString(string? connectionName = null)
    {
        var config = GetConnectionConfig(connectionName);
        return config.ConnectionString;
    }

    protected abstract IDbConnection CreateConnectionCore(DbConnectionConfig config);

    private DbConnectionConfig GetConnectionConfig(string? connectionName)
    {
        var name = string.IsNullOrWhiteSpace(connectionName) ? _defaultConnectionName : connectionName;

        var config = string.IsNullOrWhiteSpace(name)
            ? _options.DefaultConnection
            : _options.Connections.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (config == null)
        {
            throw new InvalidOperationException(
                string.IsNullOrWhiteSpace(name)
                    ? "No default connection configured."
                    : $"Connection '{name}' not found.");
        }

        return config;
    }
}
