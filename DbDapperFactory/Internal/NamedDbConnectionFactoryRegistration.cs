using System.Data.Common;

namespace DbDapperFactory;

internal interface INamedDbConnectionFactoryRegistration
{
    string Name { get; }
    DbConnection Create(IServiceProvider serviceProvider);
}

internal sealed class DelegateNamedDbConnectionFactoryRegistration : INamedDbConnectionFactoryRegistration
{
    private readonly Func<IServiceProvider, DbConnection> _factory;

    public DelegateNamedDbConnectionFactoryRegistration(
        string name,
        Func<IServiceProvider, DbConnection> factory)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Connection name cannot be null/empty.", nameof(name));
        }

        Name = name;
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public string Name { get; }

    public DbConnection Create(IServiceProvider serviceProvider) => _factory(serviceProvider);
}
