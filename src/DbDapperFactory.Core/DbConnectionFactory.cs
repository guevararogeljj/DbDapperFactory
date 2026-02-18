using System.Data;

namespace DbDapperFactory.Core;

/// <summary>
/// Generic database connection factory that uses a factory function to create connections.
/// </summary>
public class DbConnectionFactory : DbConnectionFactoryBase
{
    private readonly Func<DbConnectionConfig, IDbConnection> _connectionFactory;

    public DbConnectionFactory(
        DbConnectionFactoryOptions options,
        Func<DbConnectionConfig, IDbConnection> connectionFactory)
        : base(options)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    protected override IDbConnection CreateConnectionCore(DbConnectionConfig config)
    {
        var connection = _connectionFactory(config);
        
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }

        return connection;
    }
}
