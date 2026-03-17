namespace DbDapperFactory;

public sealed class DapperConnectionFactoryOptions
{
    public const string SectionName = "DbDapperFactory";

    public List<DapperConnectionRegistrationOptions> Connections { get; init; } = [];
}

public sealed class DapperConnectionRegistrationOptions
{
    public string Name { get; init; } = string.Empty;

    public DatabaseProvider Provider { get; init; }

    public string? ConnectionString { get; init; }

    public string? ConnectionStringName { get; init; }
}

