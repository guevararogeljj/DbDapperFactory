using System.Data.Common;
using DbDapperFactory;
using MySqlConnector;

namespace DbDapperFactory.Providers;

public static class MySqlExtensions
{
    public static IDapperConnectionFactoryBuilder AddMySql(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        string connectionString,
        Action<MySqlConnection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null/empty.", nameof(connectionString));
        }

        return builder.Add(name, _ =>
        {
            var conn = new MySqlConnection(connectionString);
            configure?.Invoke(conn);
            return conn;
        });
    }
}

