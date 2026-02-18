using System.Data.Common;

namespace DbDapperFactory;

public interface IDapperConnectionFactoryBuilder
{
    IDapperConnectionFactoryBuilder Add(string name, Func<IServiceProvider, DbConnection> connectionFactory);
}
