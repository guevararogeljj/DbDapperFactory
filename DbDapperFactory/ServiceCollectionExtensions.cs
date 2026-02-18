using Microsoft.Extensions.DependencyInjection;

namespace DbDapperFactory;

public static class ServiceCollectionExtensions
{
    public static IDapperConnectionFactoryBuilder AddDapperConnectionFactory(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IDapperConnectionFactory, DapperConnectionFactory>();
        return new DapperConnectionFactoryBuilder(services);
    }
}
