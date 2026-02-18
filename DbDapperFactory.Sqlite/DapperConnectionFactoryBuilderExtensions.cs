using DbDapperFactory;
using Microsoft.Data.Sqlite;

namespace DbDapperFactory;

public static class DapperConnectionFactoryBuilderExtensions
{
    public static IDapperConnectionFactoryBuilder AddSqlite(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        string connectionString,
        Action<SqliteConnection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null/empty.", nameof(connectionString));
        }

        return builder.Add(name, _ =>
        {
            var conn = new SqliteConnection(connectionString);
            configure?.Invoke(conn);
            return conn;
        });
    }
}
