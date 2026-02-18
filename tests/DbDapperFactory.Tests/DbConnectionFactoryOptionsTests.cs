using DbDapperFactory.Core;
using Xunit;

namespace DbDapperFactory.Tests;

public class DbConnectionFactoryOptionsTests
{
    [Fact]
    public void DefaultConnection_ReturnsFirstMarkedAsDefault()
    {
        var options = new DbConnectionFactoryOptions
        {
            Connections = new List<DbConnectionConfig>
            {
                new() { Name = "DB1", ConnectionString = "conn1", ProviderName = "SqlServer", IsDefault = false },
                new() { Name = "DB2", ConnectionString = "conn2", ProviderName = "SqlServer", IsDefault = true },
                new() { Name = "DB3", ConnectionString = "conn3", ProviderName = "SqlServer", IsDefault = false }
            }
        };

        Assert.NotNull(options.DefaultConnection);
        Assert.Equal("DB2", options.DefaultConnection.Name);
    }

    [Fact]
    public void DefaultConnection_ReturnsFirstWhenNoneMarked()
    {
        var options = new DbConnectionFactoryOptions
        {
            Connections = new List<DbConnectionConfig>
            {
                new() { Name = "DB1", ConnectionString = "conn1", ProviderName = "SqlServer" },
                new() { Name = "DB2", ConnectionString = "conn2", ProviderName = "SqlServer" }
            }
        };

        Assert.NotNull(options.DefaultConnection);
        Assert.Equal("DB1", options.DefaultConnection.Name);
    }

    [Fact]
    public void DefaultConnection_ReturnsNullWhenNoConnections()
    {
        var options = new DbConnectionFactoryOptions();
        Assert.Null(options.DefaultConnection);
    }
}
