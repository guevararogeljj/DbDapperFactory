namespace DbDapperFactory.Core;

/// <summary>
/// Configuration for a database connection.
/// </summary>
public class DbConnectionConfig
{
    /// <summary>
    /// Gets or sets the name of the connection.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database provider type (SqlServer, PostgreSql, MySql, SQLite, Oracle).
    /// </summary>
    public string ProviderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this is the default connection.
    /// </summary>
    public bool IsDefault { get; set; }
}
