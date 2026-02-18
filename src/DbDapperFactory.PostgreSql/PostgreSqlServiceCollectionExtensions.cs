using DbDapperFactory.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace DbDapperFactory.PostgreSql;

/// <summary>
/// Extension methods for configuring PostgreSQL database connections.
/// </summary>
public static class PostgreSqlServiceCollectionExtensions
{
    /// <summary>
    /// Adds PostgreSQL connection factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPostgreSqlConnectionFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IDbConnectionFactory, PostgreSqlConnectionFactory>();
        return services;
    }

    /// <summary>
    /// Adds PostgreSQL connection factory with a custom connection factory function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPostgreSqlConnectionFactory(
        this IServiceCollection services,
        Func<DbConnectionConfig, NpgsqlConnection> connectionFactory)
    {
        services.TryAddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<DbConnectionFactoryOptions>();
            return new DbConnectionFactory(options, config => connectionFactory(config));
        });
        return services;
    }
}
