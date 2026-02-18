using DbDapperFactory;
using Npgsql;

namespace DbDapperFactory;

public static class DapperConnectionFactoryBuilderExtensions
{
    public static IDapperConnectionFactoryBuilder AddPostgres(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        string connectionString,
        Action<NpgsqlConnection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null/empty.", nameof(connectionString));
        }

        return builder.Add(name, _ =>
        {
            var conn = new NpgsqlConnection(connectionString);
            configure?.Invoke(conn);
            return conn;
        });
    }
}
