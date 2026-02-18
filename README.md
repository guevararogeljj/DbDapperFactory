# DbDapperFactory

Factory + extensiones de DI para crear `DbConnection` nombradas y usarlas con Dapper.

## ¿Qué es DbDapperFactory?

**DbDapperFactory** es una librería que facilita la integración de múltiples bases de datos en aplicaciones .NET usando **Dapper** como ORM ligero. Proporciona:

- 🏭 **Factory de conexiones nombradas**: Define múltiples conexiones a diferentes bases de datos y accede a ellas por nombre.
- 📦 **Inyección de dependencias (DI)**: Integración nativa con `Microsoft.Extensions.DependencyInjection`.
- 🗄️ **Soporte multi-proveedor integrado**: SqlServer, PostgreSQL, MySQL, SQLite, Oracle - todo en un solo paquete.
- 🔌 **Simple y ligero**: Minimal overhead, máximo control sobre las conexiones.

## Instalación

Instala el único paquete **DbDapperFactory** que incluye soporte para todos los proveedores de bases de datos:

```bash
dotnet add package DbDapperFactory
```

O desde NuGet Package Manager.

## Uso Básico (DI + conexiones nombradas)

### 1. Configura en el startup

```csharp
using DbDapperFactory;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Registra la factory y define tus conexiones
builder.Services
    .AddDapperConnectionFactory()
    .AddSqlServer("Main", configuration.GetConnectionString("Main")!)
    .AddPostgres("Reporting", configuration.GetConnectionString("Reporting")!);

var app = builder.Build();
```

### 2. Inyecta y usa en tus servicios

```csharp
using DbDapperFactory;
using Dapper;

public sealed class UsersRepository
{
    private readonly IDapperConnectionFactory _connections;

    public UsersRepository(IDapperConnectionFactory connections)
        => _connections = connections;

    // Obtener usuario por ID
    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var conn = _connections.CreateConnection("Main");
        return await conn.QuerySingleOrDefaultAsync<User>(
            "select * from users where id = @id",
            new { id });
    }

    // Listar usuarios
    public async Task<List<User>> GetAllAsync()
    {
        using var conn = _connections.CreateConnection("Main");
        var users = await conn.QueryAsync<User>("select * from users");
        return users.ToList();
    }

    // Crear usuario
    public async Task<int> CreateAsync(User user)
    {
        using var conn = _connections.CreateConnection("Main");
        return await conn.ExecuteAsync(
            "insert into users (id, name, email) values (@id, @name, @email)",
            user);
    }
}

// Modelo
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

## Ejemplos Avanzados

### Múltiples bases de datos

Con un único paquete `DbDapperFactory`, puedes usar todos los proveedores simultáneamente:

```csharp
// Configuración - todo en un solo paquete
builder.Services
    .AddDapperConnectionFactory()
    .AddSqlServer("Main", "Server=localhost;Database=MyApp;...")
    .AddPostgres("Analytics", "Host=localhost;Database=Analytics;...")
    .AddMySql("Legacy", "Server=localhost;Database=OldApp;...");

// Uso
public class ReportingService
{
    private readonly IDapperConnectionFactory _connections;

    public ReportingService(IDapperConnectionFactory connections)
        => _connections = connections;

    public async Task<List<DailySales>> GetSalesFromAnalyticsAsync()
    {
        using var conn = _connections.CreateConnection("Analytics");
        return (await conn.QueryAsync<DailySales>(
            "select * from daily_sales where date >= @startDate",
            new { startDate = DateTime.Now.AddDays(-30) })).ToList();
    }

    public async Task<List<LegacyUser>> GetUsersFromLegacyAsync()
    {
        using var conn = _connections.CreateConnection("Legacy");
        return (await conn.QueryAsync<LegacyUser>(
            "select * from users")).ToList();
    }
}
```

### Configuración personalizada por proveedor

```csharp
// SQL Server con opciones
builder.Services
    .AddDapperConnectionFactory()
    .AddSqlServer(
        "Main",
        "Server=localhost;Database=MyApp;...",
        configure: conn =>
        {
            conn.ConnectionTimeout = 30;
        });

// PostgreSQL
builder.Services.AddPostgres(
    "Reporting",
    "Host=localhost;Database=Analytics;...");

// MySQL
builder.Services.AddMySql(
    "Users",
    "Server=localhost;Database=UsersDb;...");

// SQLite
builder.Services.AddSqlite(
    "Cache",
    "Data Source=cache.db;");

// Oracle
builder.Services.AddOracle(
    "Legacy",
    "Data Source=OracleDB;User Id=user;Password=pass;");
```

## Notas Importantes

- ✅ La factory **no** abre la conexión automáticamente. Tú controlas cuándo llamar a `Open()`/`OpenAsync()`.
- ✅ Cada llamada a `CreateConnection(name)` crea una instancia nueva de conexión.
- ✅ Usa `using` para asegurar que la conexión se cierre y disponga correctamente.
- ✅ Compatible con Dapper para queries, inserts, updates, deletes y procedimientos almacenados.

## Características

| Característica | Detalles |
|---|---|
| **Un solo paquete** | Todo incluido - SqlServer, PostgreSQL, MySQL, SQLite, Oracle |
| **Inyección de Dependencias** | Integrada con `IServiceCollection` |
| **Conexiones Nombradas** | Define múltiples conexiones y accede por nombre |
| **Async/Await** | Compatible con operaciones asincrónicas |
| **Dapper Integration** | Funciona perfectamente con Dapper |
| **Lightweight** | Minimal, sin dependencias pesadas |

## Proveedores Soportados

✅ **SQL Server** - `AddSqlServer()`  
✅ **PostgreSQL** - `AddPostgres()`  
✅ **MySQL** - `AddMySql()`  
✅ **SQLite** - `AddSqlite()`  
✅ **Oracle** - `AddOracle()`  

## Licencia

Este proyecto está bajo licencia MIT.

