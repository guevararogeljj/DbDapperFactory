namespace DbDapperFactory.Core;

/// <summary>
/// Options for configuring the database connection factory.
/// </summary>
public class DbConnectionFactoryOptions
{
    /// <summary>
    /// Gets or sets the collection of database connection configurations.
    /// </summary>
    public List<DbConnectionConfig> Connections { get; set; } = new();

    /// <summary>
    /// Gets the default connection configuration.
    /// </summary>
    public DbConnectionConfig? DefaultConnection => 
        Connections.FirstOrDefault(c => c.IsDefault) ?? Connections.FirstOrDefault();
}
