using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DbDapperFactory;

internal sealed class DapperConnectionFactoryBuilder : IDapperConnectionFactoryBuilder
{
    private readonly IServiceCollection _services;

    public DapperConnectionFactoryBuilder(IServiceCollection services)
        => _services = services ?? throw new ArgumentNullException(nameof(services));

    public IDapperConnectionFactoryBuilder Add(string name, Func<IServiceProvider, DbConnection> connectionFactory)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Connection name cannot be null/empty.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(connectionFactory);

        _services.AddSingleton<INamedDbConnectionFactoryRegistration>(
            new DelegateNamedDbConnectionFactoryRegistration(name, connectionFactory));

        return this;
    }
}
