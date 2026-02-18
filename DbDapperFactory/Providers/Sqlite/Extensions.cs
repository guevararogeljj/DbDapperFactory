using System.Data.Common;
using System.Data.SQLite;
using DbDapperFactory;

namespace DbDapperFactory.Providers;

public static class SqliteExtensions
{
    public static IDapperConnectionFactoryBuilder AddSqlite(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        string connectionString,
        Action<SQLiteConnection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null/empty.", nameof(connectionString));
        }

        return builder.Add(name, _ =>
        {
            var conn = new SQLiteConnection(connectionString);
            configure?.Invoke(conn);
            return conn;
        });
    }
}

