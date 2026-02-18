using System.Data.Common;

namespace DbDapperFactory;

public interface IDapperConnectionFactory
{
    DbConnection CreateConnection(string name);
}
