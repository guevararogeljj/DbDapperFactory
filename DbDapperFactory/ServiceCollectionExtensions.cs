using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbDapperFactory;

public static class ServiceCollectionExtensions
{
    public static IDapperConnectionFactoryBuilder AddDapperConnectionFactory(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IDapperConnectionFactory>(serviceProvider =>
            new DapperConnectionFactory(
                serviceProvider,
                serviceProvider.GetServices<INamedDbConnectionFactoryRegistration>()));

        return new DapperConnectionFactoryBuilder(services);
    }

    public static IServiceCollection AddProviderDapperConnectionFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        params (string Name, DatabaseProvider Provider)[] databases)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(databases);

        if (databases.Length == 0)
        {
            throw new ArgumentException("At least one database configuration is required.", nameof(databases));
        }

        services.AddScoped<IDapperConnectionFactory>(_ =>
            new DapperConnectionFactory(configuration, databases));

        return services;
    }

    [Obsolete("Use AddProviderDapperConnectionFactory instead.")]
    public static IServiceCollection AddMultiDbConnectionFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        params (string Name, DatabaseProvider Provider)[] databases)
        => services.AddProviderDapperConnectionFactory(configuration, databases);

    public static IDapperConnectionFactoryBuilder AddDapperConnectionFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionPath = DapperConnectionFactoryOptions.SectionName)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        return services
            .AddDapperConnectionFactory()
            .AddConfiguredConnections(configuration, sectionPath);
    }
}
