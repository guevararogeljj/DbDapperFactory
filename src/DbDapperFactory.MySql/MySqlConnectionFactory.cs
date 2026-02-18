using System.Data;
using DbDapperFactory.Core;
using MySqlConnector;

namespace DbDapperFactory.MySql;

/// <summary>
/// MySQL implementation of the database connection factory.
/// </summary>
public class MySqlConnectionFactory : DbConnectionFactoryBase
{
    public MySqlConnectionFactory(DbConnectionFactoryOptions options) : base(options)
    {
    }

    protected override IDbConnection CreateConnectionCore(DbConnectionConfig config)
    {
        var connection = new MySqlConnection(config.ConnectionString);
        connection.Open();
        return connection;
    }
}
