using System.Data;
using DbDapperFactory.Core;
using Npgsql;

namespace DbDapperFactory.PostgreSql;

/// <summary>
/// PostgreSQL implementation of the database connection factory.
/// </summary>
public class PostgreSqlConnectionFactory : DbConnectionFactoryBase
{
    public PostgreSqlConnectionFactory(DbConnectionFactoryOptions options) : base(options)
    {
    }

    protected override IDbConnection CreateConnectionCore(DbConnectionConfig config)
    {
        var connection = new NpgsqlConnection(config.ConnectionString);
        connection.Open();
        return connection;
    }
}
