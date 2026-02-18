using DbDapperFactory.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySqlConnector;

namespace DbDapperFactory.MySql;

/// <summary>
/// Extension methods for configuring MySQL database connections.
/// </summary>
public static class MySqlServiceCollectionExtensions
{
    /// <summary>
    /// Adds MySQL connection factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMySqlConnectionFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
        return services;
    }

    /// <summary>
    /// Adds MySQL connection factory with a custom connection factory function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMySqlConnectionFactory(
        this IServiceCollection services,
        Func<DbConnectionConfig, MySqlConnection> connectionFactory)
    {
        services.TryAddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<DbConnectionFactoryOptions>();
            return new DbConnectionFactory(options, config => connectionFactory(config));
        });
        return services;
    }
}
