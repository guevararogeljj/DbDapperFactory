# DbDapperFactory

[Español](./README.md) | [English](./README.en.md)

Factory and DI extensions to create named `DbConnection` instances and use them with Dapper.

## What this package is

`DbDapperFactory` is a package for .NET applications that simplifies registration and creation of connections to multiple databases when you work with Dapper.

It helps you:

- register multiple connections by name
- associate each connection with a specific provider
- resolve connections through dependency injection
- keep a single place responsible for creating `DbConnection`

The package currently includes support for:

- SQL Server
- PostgreSQL
- MySQL
- SQLite
- Oracle

## Single package

The previous provider-specific packages were consolidated into a single package:

- `DbDapperFactory.Core`
- `DbDapperFactory.SqlServer`
- `DbDapperFactory.Postgres`
- `DbDapperFactory.MySql`
- `DbDapperFactory.Sqlite`
- `DbDapperFactory.Oracle`

Everything is now installed from:

```bash
dotnet add package DbDapperFactory
```

## When to use it

This package is useful when your application:

- consumes one or more databases
- needs to separate connections by name, for example `Main`, `Reporting`, `Legacy`
- uses Dapper and you want to centralize connection creation
- needs to switch between providers without rewriting all data access infrastructure

## Main API

### `IDapperConnectionFactory`

The main interface exposes a single method:

```csharp
public interface IDapperConnectionFactory
{
    DbConnection CreateConnection(string name);
}
```

Important points:

- `CreateConnection(name)` returns a new connection every time
- the factory does not open the connection automatically
- you control when to call `Open()` or `OpenAsync()`
- the connection should be disposed with `using`

## Supported providers

The current enum is:

```csharp
public enum DatabaseProvider
{
    SqlServer,
    Postgres,
    MySql,
    Sqlite,
    Oracle
}
```

## Available registration options

The library supports three main ways to register connections.

### Option 1: fluent registration by provider

Use this option when you want to define each connection explicitly in code.

> Important: `AddSqlServer`, `AddPostgres`, `AddMySql`, `AddSqlite`, and `AddOracle` live in the `DbDapperFactory.Providers` namespace.

```csharp
using DbDapperFactory;
using DbDapperFactory.Providers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDapperConnectionFactory()
    .AddSqlServer("Main", builder.Configuration.GetConnectionString("Main")!)
    .AddPostgres("Reporting", builder.Configuration.GetConnectionString("Reporting")!)
    .AddMySql("Legacy", builder.Configuration.GetConnectionString("Legacy")!);
```

You can also use the remaining providers:

```csharp
builder.Services
    .AddDapperConnectionFactory()
    .AddSqlite("Cache", "Data Source=cache.db;")
    .AddOracle("ERP", "User Id=user;Password=pass;Data Source=MyOracleDb");
```

### Option 2: registration by name + `DatabaseProvider`

Use this option when you want to read connection strings from `ConnectionStrings` and only specify in code which provider belongs to each name.

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost;Database=Documents;User Id=sa;Password=YourPassword;TrustServerCertificate=True",
    "PostgresConnection": "Host=localhost;Port=5432;Database=documents;Username=postgres;Password=secret;Search Path=public"
  }
}
```

Registration:

```csharp
using DbDapperFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

services.AddProviderDapperConnectionFactory(
    configuration,
    ("DefaultConnection", DatabaseProvider.SqlServer),
    ("PostgresConnection", DatabaseProvider.Postgres));
```

The library will look up each string in `ConnectionStrings:{Name}` and create the proper connection based on the specified `DatabaseProvider`.

> `AddMultiDbConnectionFactory` still exists as an obsolete alias, but the recommended API is `AddProviderDapperConnectionFactory`.

### Option 3: registration by configuration section

Use this option when you also want to declare the provider in configuration.

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MainDb": "Server=localhost;Database=AppDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True",
    "ReportingDb": "Host=localhost;Port=5432;Database=reporting;Username=postgres;Password=secret"
  },
  "DbDapperFactory": {
    "Connections": [
      {
        "Name": "Main",
        "Provider": "SqlServer",
        "ConnectionStringName": "MainDb"
      },
      {
        "Name": "Reporting",
        "Provider": "Postgres",
        "ConnectionStringName": "ReportingDb"
      },
      {
        "Name": "Cache",
        "Provider": "Sqlite",
        "ConnectionString": "Data Source=cache.db;"
      }
    ]
  }
}
```

Registration:

```csharp
using DbDapperFactory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDapperConnectionFactory(builder.Configuration);
```

The default section is `DbDapperFactory`.

Each connection can resolve its string in these ways:

- `ConnectionString`: direct string
- `ConnectionStringName`: name under `ConnectionStrings`
- if both are omitted, it tries to use `Name` as the key under `ConnectionStrings`

## Example usage with Dapper

```csharp
using DbDapperFactory;
using Dapper;

public sealed class UsersRepository
{
    private readonly IDapperConnectionFactory _connections;

    public UsersRepository(IDapperConnectionFactory connections)
        => _connections = connections;

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var conn = _connections.CreateConnection("Main");
        await conn.OpenAsync();

        return await conn.QuerySingleOrDefaultAsync<User>(
            "select * from users where id = @id",
            new { id });
    }

    public async Task<List<User>> GetAllAsync()
    {
        using var conn = _connections.CreateConnection("Main");
        await conn.OpenAsync();

        var users = await conn.QueryAsync<User>("select * from users");
        return users.ToList();
    }

    public async Task<int> CreateAsync(User user)
    {
        using var conn = _connections.CreateConnection("Main");
        await conn.OpenAsync();

        return await conn.ExecuteAsync(
            "insert into users (id, name, email) values (@id, @name, @email)",
            user);
    }
}

public sealed class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
```

## Example with multiple databases

```csharp
using DbDapperFactory;
using DbDapperFactory.Providers;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDapperConnectionFactory()
    .AddSqlServer("Operational", builder.Configuration.GetConnectionString("Operational")!)
    .AddPostgres("Analytics", builder.Configuration.GetConnectionString("Analytics")!);

public sealed class ReportingService
{
    private readonly IDapperConnectionFactory _connections;

    public ReportingService(IDapperConnectionFactory connections)
        => _connections = connections;

    public async Task<IReadOnlyList<OrderSummary>> GetOperationalOrdersAsync()
    {
        using var conn = _connections.CreateConnection("Operational");
        await conn.OpenAsync();

        var rows = await conn.QueryAsync<OrderSummary>(
            "select top 10 Id, CustomerName, Total from dbo.Orders order by CreatedAt desc");

        return rows.AsList();
    }

    public async Task<IReadOnlyList<DailySales>> GetAnalyticsAsync()
    {
        using var conn = _connections.CreateConnection("Analytics");
        await conn.OpenAsync();

        var rows = await conn.QueryAsync<DailySales>(
            "select day, total from public.daily_sales order by day desc limit 10");

        return rows.AsList();
    }
}
```

## Quick package example

If you want a short reference for what the package is for, this is a typical use case:

```csharp
using DbDapperFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddProviderDapperConnectionFactory(
    configuration,
    ("SqlDb", DatabaseProvider.SqlServer),
    ("PgDb", DatabaseProvider.Postgres));

var provider = services.BuildServiceProvider();
var factory = provider.GetRequiredService<IDapperConnectionFactory>();

using var sqlConnection = factory.CreateConnection("SqlDb");
using var pgConnection = factory.CreateConnection("PgDb");
```

This example shows the package goal: create the correct connection by name and provider without forcing your consumer code to manually instantiate `SqlConnection`, `NpgsqlConnection`, `MySqlConnection`, `SQLiteConnection`, or `OracleConnection` in different places.

## Important notes

- `CreateConnection(name)` creates a new instance every time.
- The factory does not open connections automatically.
- The library works well with Dapper, but it can also be used with plain ADO.NET.
- If you register duplicate connection names with the fluent builder, the factory will throw when resolving them.
- If you use `AddProviderDapperConnectionFactory`, each name must exist in `ConnectionStrings`.

## Migration from previous versions

If you were still using separate packages by provider, migrate like this:

### 1. Remove old packages

```bash
dotnet remove package DbDapperFactory.Core
dotnet remove package DbDapperFactory.SqlServer
dotnet remove package DbDapperFactory.Postgres
dotnet remove package DbDapperFactory.MySql
dotnet remove package DbDapperFactory.Sqlite
dotnet remove package DbDapperFactory.Oracle
```

### 2. Install the single package

```bash
dotnet add package DbDapperFactory
```

### 3. Update `using` directives if you use provider-specific methods

```csharp
using DbDapperFactory;
using DbDapperFactory.Providers;
```

## 👤 Author

guevararogeljj

## License

This project is licensed under the MIT License.

