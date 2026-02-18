using System.Data;
using DbDapperFactory.Core;
using Microsoft.Data.SqlClient;

namespace DbDapperFactory.SqlServer;

/// <summary>
/// SQL Server implementation of the database connection factory.
/// </summary>
public class SqlServerConnectionFactory : DbConnectionFactoryBase
{
    public SqlServerConnectionFactory(DbConnectionFactoryOptions options) : base(options)
    {
    }

    protected override IDbConnection CreateConnectionCore(DbConnectionConfig config)
    {
        var connection = new SqlConnection(config.ConnectionString);
        connection.Open();
        return connection;
    }
}
