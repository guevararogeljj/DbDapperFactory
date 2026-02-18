using DbDapperFactory.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbDapperFactory.SQLite;

/// <summary>
/// Extension methods for configuring SQLite database connections.
/// </summary>
public static class SQLiteServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQLite connection factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSQLiteConnectionFactory(this IServiceCollection services)
    {
        services.TryAddSingleton<IDbConnectionFactory, SQLiteConnectionFactory>();
        return services;
    }

    /// <summary>
    /// Adds SQLite connection factory with a custom connection factory function.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSQLiteConnectionFactory(
        this IServiceCollection services,
        Func<DbConnectionConfig, SqliteConnection> connectionFactory)
    {
        services.TryAddSingleton<IDbConnectionFactory>(sp =>
        {
            var options = sp.GetRequiredService<DbConnectionFactoryOptions>();
            return new DbConnectionFactory(options, config => connectionFactory(config));
        });
        return services;
    }
}
