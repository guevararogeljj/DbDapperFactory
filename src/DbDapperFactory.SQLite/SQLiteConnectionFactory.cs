using System.Data;
using DbDapperFactory.Core;
using Microsoft.Data.Sqlite;

namespace DbDapperFactory.SQLite;

/// <summary>
/// SQLite implementation of the database connection factory.
/// </summary>
public class SQLiteConnectionFactory : DbConnectionFactoryBase
{
    public SQLiteConnectionFactory(DbConnectionFactoryOptions options) : base(options)
    {
    }

    protected override IDbConnection CreateConnectionCore(DbConnectionConfig config)
    {
        var connection = new SqliteConnection(config.ConnectionString);
        connection.Open();
        return connection;
    }
}
