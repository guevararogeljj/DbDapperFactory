using System.Data.Common;
using DbDapperFactory;
using Oracle.ManagedDataAccess.Client;

namespace DbDapperFactory.Providers;

public static class OracleExtensions
{
    public static IDapperConnectionFactoryBuilder AddOracle(
        this IDapperConnectionFactoryBuilder builder,
        string name,
        string connectionString,
        Action<OracleConnection>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null/empty.", nameof(connectionString));
        }

        return builder.Add(name, _ =>
        {
            var conn = new OracleConnection(connectionString);
            configure?.Invoke(conn);
            return conn;
        });
    }
}

