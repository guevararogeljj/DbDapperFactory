using System.Data.Common;
using DbDapperFactory;
using Microsoft.Data.SqlClient;

namespace DbDapperFactory.Providers;

public static class SqlServerExtensions
{
    public static IDapperConnectionFactoryBuilder AddSqlServer(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        string connectionString,
        Action<SqlConnection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null/empty.", nameof(connectionString));
        }

        return builder.Add(name, _ =>
        {
            var conn = new SqlConnection(connectionString);
            configure?.Invoke(conn);
            return conn;
        });
    }
}

