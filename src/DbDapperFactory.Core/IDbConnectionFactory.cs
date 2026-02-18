using System.Data;

namespace DbDapperFactory.Core;

/// <summary>
/// Factory interface for creating named database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a database connection for the specified named connection.
    /// </summary>
    /// <param name="connectionName">The name of the connection to create. If null or empty, uses the default connection.</param>
    /// <returns>An open IDbConnection instance.</returns>
    IDbConnection CreateConnection(string? connectionName = null);

    /// <summary>
    /// Gets the connection string for the specified named connection.
    /// </summary>
    /// <param name="connectionName">The name of the connection. If null or empty, uses the default connection.</param>
    /// <returns>The connection string.</returns>
    string GetConnectionString(string? connectionName = null);
}
