using DbDapperFactory.Providers;
using Microsoft.Extensions.Configuration;

namespace DbDapperFactory;

public static class DapperConnectionFactoryBuilderExtensions
{
    public static IDapperConnectionFactoryBuilder AddDatabase(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        DatabaseProvider provider,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return provider switch
        {
            DatabaseProvider.SqlServer => builder.AddSqlServer(name, connectionString),
            DatabaseProvider.Postgres => builder.AddPostgres(name, connectionString),
            DatabaseProvider.MySql => builder.AddMySql(name, connectionString),
            DatabaseProvider.Sqlite => builder.AddSqlite(name, connectionString),
            DatabaseProvider.Oracle => builder.AddOracle(name, connectionString),
            _ => throw new NotSupportedException($"Provider '{provider}' is not supported.")
        };
    }

    public static IDapperConnectionFactoryBuilder AddConfiguredConnections(
        this IDapperConnectionFactoryBuilder builder,
        IConfiguration configuration,
        string sectionPath = DapperConnectionFactoryOptions.SectionName)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        if (string.IsNullOrWhiteSpace(sectionPath))
        {
            throw new ArgumentException("Configuration section path cannot be null/empty.", nameof(sectionPath));
        }

        var section = configuration.GetSection(sectionPath);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{sectionPath}' was not found.");
        }

        var options = section.Get<DapperConnectionFactoryOptions>();
        if (options?.Connections is null || options.Connections.Count == 0)
        {
            throw new InvalidOperationException(
                $"Configuration section '{sectionPath}' must contain at least one entry in '{nameof(DapperConnectionFactoryOptions.Connections)}'.");
        }

        foreach (var connection in options.Connections)
        {
            var connectionString = ResolveConnectionString(configuration, connection);
            builder.AddDatabase(connection.Name, connection.Provider, connectionString);
        }

        return builder;
    }

    private static string ResolveConnectionString(
        IConfiguration configuration,
        DapperConnectionRegistrationOptions connection)
    {
        if (string.IsNullOrWhiteSpace(connection.Name))
        {
            throw new InvalidOperationException("Each configured database must define a non-empty Name.");
        }

        if (!string.IsNullOrWhiteSpace(connection.ConnectionString))
        {
            return connection.ConnectionString;
        }

        if (!string.IsNullOrWhiteSpace(connection.ConnectionStringName))
        {
            var namedConnectionString = configuration.GetConnectionString(connection.ConnectionStringName);
            if (!string.IsNullOrWhiteSpace(namedConnectionString))
            {
                return namedConnectionString;
            }

            throw new InvalidOperationException(
                $"Connection string '{connection.ConnectionStringName}' was not found for database '{connection.Name}'.");
        }

        var connectionString = configuration.GetConnectionString(connection.Name);
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        throw new InvalidOperationException(
            $"Database '{connection.Name}' must define '{nameof(DapperConnectionRegistrationOptions.ConnectionString)}', '{nameof(DapperConnectionRegistrationOptions.ConnectionStringName)}', or a matching entry under 'ConnectionStrings'.");
    }
}

