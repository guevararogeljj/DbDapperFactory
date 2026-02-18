using System.Data;
using DbDapperFactory.Core;
using Oracle.ManagedDataAccess.Client;

namespace DbDapperFactory.Oracle;

/// <summary>
/// Oracle implementation of the database connection factory.
/// </summary>
public class OracleConnectionFactory : DbConnectionFactoryBase
{
    public OracleConnectionFactory(DbConnectionFactoryOptions options) : base(options)
    {
    }

    protected override IDbConnection CreateConnectionCore(DbConnectionConfig config)
    {
        var connection = new OracleConnection(config.ConnectionString);
        connection.Open();
        return connection;
    }
}
