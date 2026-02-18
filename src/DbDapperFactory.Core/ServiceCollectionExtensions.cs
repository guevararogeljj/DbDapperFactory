using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DbDapperFactory.Core;

/// <summary>
/// Extension methods for configuring database connection factory services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a database connection factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section containing connection strings.</param>
    /// <param name="sectionName">The name of the configuration section (default: "ConnectionStrings").</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "ConnectionStrings")
    {
        var options = new DbConnectionFactoryOptions();
        configuration.GetSection(sectionName).Bind(options);

        services.TryAddSingleton(options);
        
        return services;
    }

    /// <summary>
    /// Adds a database connection factory to the service collection with custom options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">An action to configure the options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddDbConnectionFactory(
        this IServiceCollection services,
        Action<DbConnectionFactoryOptions> configureOptions)
    {
        var options = new DbConnectionFactoryOptions();
        configureOptions(options);

        services.TryAddSingleton(options);
        
        return services;
    }
}
