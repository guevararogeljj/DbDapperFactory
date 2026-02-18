using DbDapperFactory.Core;
using DbDapperFactory.SQLite;
using Microsoft.Data.Sqlite;
using Xunit;

namespace DbDapperFactory.Tests;

public class SQLiteConnectionFactoryTests : IDisposable
{
    private readonly string _connectionString;
    private readonly DbConnectionFactoryOptions _options;

    public SQLiteConnectionFactoryTests()
    {
        _connectionString = "Data Source=:memory:";
        _options = new DbConnectionFactoryOptions
        {
            Connections = new List<DbConnectionConfig>
            {
                new()
                {
                    Name = "Default",
                    ConnectionString = _connectionString,
                    ProviderName = "SQLite",
                    IsDefault = true
                },
                new()
                {
                    Name = "Secondary",
                    ConnectionString = _connectionString,
                    ProviderName = "SQLite"
                }
            }
        };
    }

    [Fact]
    public void CreateConnection_WithoutName_ReturnsDefaultConnection()
    {
        var factory = new SQLiteConnectionFactory(_options);
        using var connection = factory.CreateConnection();

        Assert.NotNull(connection);
        Assert.IsType<SqliteConnection>(connection);
        Assert.Equal(System.Data.ConnectionState.Open, connection.State);
    }

    [Fact]
    public void CreateConnection_WithName_ReturnsNamedConnection()
    {
        var factory = new SQLiteConnectionFactory(_options);
        using var connection = factory.CreateConnection("Secondary");

        Assert.NotNull(connection);
        Assert.IsType<SqliteConnection>(connection);
        Assert.Equal(System.Data.ConnectionState.Open, connection.State);
    }

    [Fact]
    public void CreateConnection_WithInvalidName_ThrowsException()
    {
        var factory = new SQLiteConnectionFactory(_options);
        Assert.Throws<InvalidOperationException>(() => factory.CreateConnection("NonExistent"));
    }

    [Fact]
    public void GetConnectionString_WithoutName_ReturnsDefaultConnectionString()
    {
        var factory = new SQLiteConnectionFactory(_options);
        var connectionString = factory.GetConnectionString();

        Assert.Equal(_connectionString, connectionString);
    }

    [Fact]
    public void GetConnectionString_WithName_ReturnsNamedConnectionString()
    {
        var factory = new SQLiteConnectionFactory(_options);
        var connectionString = factory.GetConnectionString("Secondary");

        Assert.Equal(_connectionString, connectionString);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
    }
}
