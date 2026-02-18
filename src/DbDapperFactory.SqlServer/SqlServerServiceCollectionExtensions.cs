using DbDapperFactory.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbDapperFactory.SqlServer;

/// <summary>
/// Extension methods for configuring SQL Server database connections.
/// </summary>
public static class SqlServerServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQL Server connection factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSqlServerConnectionFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();
        return services;
    }

    /// <summary>
    /// Adds SQL Server connection factory with a custom connection factory function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSqlServerConnectionFactory(
        this IServiceCollection services,
        Func<DbConnectionConfig, SqlConnection> connectionFactory)
    {
        services.TryAddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<DbConnectionFactoryOptions>();
            return new DbConnectionFactory(options, config => connectionFactory(config));
        });
        return services;
    }
}
