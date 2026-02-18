using DbDapperFactory.Core;
using DbDapperFactory.SQLite;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DbDapperFactory.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDbConnectionFactory_WithAction_RegistersOptions()
    {
        var services = new ServiceCollection();
        
        services.AddDbConnectionFactory(options =>
        {
            options.Connections.Add(new DbConnectionConfig
            {
                Name = "Test",
                ConnectionString = "Data Source=:memory:",
                ProviderName = "SQLite",
                IsDefault = true
            });
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<DbConnectionFactoryOptions>();

        Assert.NotNull(options);
        Assert.Single(options.Connections);
        Assert.Equal("Test", options.Connections[0].Name);
    }

    [Fact]
    public void AddSQLiteConnectionFactory_RegistersFactory()
    {
        var services = new ServiceCollection();
        
        services.AddDbConnectionFactory(options =>
        {
            options.Connections.Add(new DbConnectionConfig
            {
                Name = "Test",
                ConnectionString = "Data Source=:memory:",
                ProviderName = "SQLite",
                IsDefault = true
            });
        });
        services.AddSQLiteConnectionFactory();

        var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IDbConnectionFactory>();

        Assert.NotNull(factory);
        Assert.IsType<SQLiteConnectionFactory>(factory);
    }
}
