using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DbDapperFactory;

internal sealed class DapperConnectionFactory : IDapperConnectionFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyDictionary<string, INamedDbConnectionFactoryRegistration> _registrations;

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

    public DbConnection CreateConnection(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Connection name cannot be null/empty.", nameof(name));
        }

        if (_registrations.TryGetValue(name, out var registration))
        {
            return registration.Create(_serviceProvider);
        }

        var available = _registrations.Keys.OrderBy(x => x).ToArray();
        var availableText = available.Length == 0 ? "<none>" : string.Join(", ", available);
        throw new KeyNotFoundException($"No Dapper connection registered with name '{name}'. Available: {availableText}.");
    }
}
