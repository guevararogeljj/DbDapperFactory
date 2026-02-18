using DbDapperFactory.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Oracle.ManagedDataAccess.Client;

namespace DbDapperFactory.Oracle;

/// <summary>
/// Extension methods for configuring Oracle database connections.
/// </summary>
public static class OracleServiceCollectionExtensions
{
    /// <summary>
    /// Adds Oracle connection factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOracleConnectionFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IDbConnectionFactory, OracleConnectionFactory>();
        return services;
    }

    /// <summary>
    /// Adds Oracle connection factory with a custom connection factory function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOracleConnectionFactory(
        this IServiceCollection services,
        Func<DbConnectionConfig, OracleConnection> connectionFactory)
    {
        services.TryAddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<DbConnectionFactoryOptions>();
            return new DbConnectionFactory(options, config => connectionFactory(config));
        });
        return services;
    }
}
