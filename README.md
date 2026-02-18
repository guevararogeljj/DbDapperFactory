# DbDapperFactory

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/github/license/guevararogeljj/DbDapperFactory)](LICENSE)

## ¿Qué es DbDapperFactory?

DbDapperFactory es un conjunto de librerías que facilita la integración de múltiples bases de datos en aplicaciones .NET usando Dapper como ORM ligero. Proporciona:

- 🏭 **Factory de conexiones nombradas**: Define múltiples conexiones a diferentes bases de datos y accede a ellas por nombre.
- 📦 **Inyección de dependencias (DI)**: Integración nativa con Microsoft.Extensions.DependencyInjection.
- 🗄️ **Soporte multi-proveedor**: SqlServer, PostgreSQL, MySQL, SQLite, Oracle.
- 🔌 **Simple y ligero**: Minimal overhead, máximo control sobre las conexiones.

## Características

- ✅ Gestión centralizada de conexiones a bases de datos
- ✅ Soporte para múltiples proveedores de bases de datos
- ✅ Configuración mediante appsettings.json o código
- ✅ Integración perfecta con Dependency Injection
- ✅ Compatible con .NET 8.0+
- ✅ Uso de Dapper para operaciones de base de datos eficientes

## Instalación

Instala el paquete core y los proveedores que necesites:

```bash
# Core library (requerido)
dotnet add package DbDapperFactory.Core

# Proveedores específicos (elige los que necesites)
dotnet add package DbDapperFactory.SqlServer
dotnet add package DbDapperFactory.PostgreSql
dotnet add package DbDapperFactory.MySql
dotnet add package DbDapperFactory.SQLite
dotnet add package DbDapperFactory.Oracle
```

## Uso Básico

### 1. Configuración en appsettings.json

```json
{
  "ConnectionStrings": {
    "Connections": [
      {
        "Name": "Primary",
        "ConnectionString": "Server=localhost;Database=MyDb;Integrated Security=true;",
        "ProviderName": "SqlServer",
        "IsDefault": true
      },
      {
        "Name": "Reporting",
        "ConnectionString": "Server=localhost;Database=ReportingDb;Integrated Security=true;",
        "ProviderName": "SqlServer"
      }
    ]
  }
}
```

### 2. Registro en Startup/Program.cs

```csharp
using DbDapperFactory.Core;
using DbDapperFactory.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Registrar la configuración y el factory
builder.Services.AddDbConnectionFactory(
    builder.Configuration, 
    "ConnectionStrings"
);

// Registrar el proveedor específico
builder.Services.AddSqlServerConnectionFactory();

var app = builder.Build();
```

### 3. Uso en tu código

```csharp
public class UserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        // Usa la conexión por defecto
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<User>("SELECT * FROM Users");
    }

    public async Task<IEnumerable<Report>> GetReportsAsync()
    {
        // Usa una conexión específica
        using var connection = _connectionFactory.CreateConnection("Reporting");
        return await connection.QueryAsync<Report>("SELECT * FROM Reports");
    }
}
```

## Configuración Avanzada

### Configuración mediante código

```csharp
builder.Services.AddDbConnectionFactory(options =>
{
    options.Connections.Add(new DbConnectionConfig
    {
        Name = "Primary",
        ConnectionString = "Server=localhost;Database=MyDb;Integrated Security=true;",
        ProviderName = "SqlServer",
        IsDefault = true
    });
    
    options.Connections.Add(new DbConnectionConfig
    {
        Name = "Cache",
        ConnectionString = "Data Source=cache.db",
        ProviderName = "SQLite"
    });
});
```

### Múltiples proveedores

```csharp
using DbDapperFactory.Core;
using DbDapperFactory.SqlServer;
using DbDapperFactory.PostgreSql;
using DbDapperFactory.SQLite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbConnectionFactory(options =>
{
    // SQL Server para datos transaccionales
    options.Connections.Add(new DbConnectionConfig
    {
        Name = "SqlServer",
        ConnectionString = "Server=localhost;Database=TransactionalDb;...",
        ProviderName = "SqlServer",
        IsDefault = true
    });
    
    // PostgreSQL para analytics
    options.Connections.Add(new DbConnectionConfig
    {
        Name = "Analytics",
        ConnectionString = "Host=localhost;Database=analytics;...",
        ProviderName = "PostgreSql"
    });
    
    // SQLite para caché local
    options.Connections.Add(new DbConnectionConfig
    {
        Name = "LocalCache",
        ConnectionString = "Data Source=cache.db",
        ProviderName = "SQLite"
    });
});

// Nota: Con múltiples proveedores, usa el factory genérico
builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    var options = sp.GetRequiredService<DbConnectionFactoryOptions>();
    return new DbConnectionFactory(options, config =>
    {
        return config.ProviderName switch
        {
            "SqlServer" => new SqlConnection(config.ConnectionString),
            "PostgreSql" => new NpgsqlConnection(config.ConnectionString),
            "SQLite" => new SqliteConnection(config.ConnectionString),
            _ => throw new NotSupportedException($"Provider {config.ProviderName} not supported")
        };
    });
});
```

## Ejemplos con Dapper

### Consultas básicas

```csharp
public class ProductRepository
{
    private readonly IDbConnectionFactory _factory;

    public ProductRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        using var connection = _factory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Product>(
            "SELECT * FROM Products WHERE Id = @Id",
            new { Id = id }
        );
    }

    public async Task<int> CreateProductAsync(Product product)
    {
        using var connection = _factory.CreateConnection();
        return await connection.ExecuteAsync(
            "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)",
            product
        );
    }
}
```

### Transacciones

```csharp
public async Task TransferFundsAsync(int fromAccountId, int toAccountId, decimal amount)
{
    using var connection = _factory.CreateConnection();
    using var transaction = connection.BeginTransaction();
    
    try
    {
        await connection.ExecuteAsync(
            "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @Id",
            new { Amount = amount, Id = fromAccountId },
            transaction
        );
        
        await connection.ExecuteAsync(
            "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @Id",
            new { Amount = amount, Id = toAccountId },
            transaction
        );
        
        transaction.Commit();
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}
```

## Proveedores Soportados

| Proveedor | Paquete | Connection String Example |
|-----------|---------|---------------------------|
| SQL Server | `DbDapperFactory.SqlServer` | `Server=localhost;Database=MyDb;Integrated Security=true;` |
| PostgreSQL | `DbDapperFactory.PostgreSql` | `Host=localhost;Database=mydb;Username=user;Password=pass;` |
| MySQL | `DbDapperFactory.MySql` | `Server=localhost;Database=mydb;Uid=user;Pwd=pass;` |
| SQLite | `DbDapperFactory.SQLite` | `Data Source=mydb.db` |
| Oracle | `DbDapperFactory.Oracle` | `Data Source=localhost:1521/orcl;User Id=user;Password=pass;` |

## Estructura del Proyecto

```
DbDapperFactory/
├── src/
│   ├── DbDapperFactory.Core/          # Biblioteca principal con abstracciones
│   ├── DbDapperFactory.SqlServer/     # Implementación para SQL Server
│   ├── DbDapperFactory.PostgreSql/    # Implementación para PostgreSQL
│   ├── DbDapperFactory.MySql/         # Implementación para MySQL
│   ├── DbDapperFactory.SQLite/        # Implementación para SQLite
│   └── DbDapperFactory.Oracle/        # Implementación para Oracle
└── tests/
    └── DbDapperFactory.Tests/         # Tests unitarios
```

## Contribuir

Las contribuciones son bienvenidas! Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## Autor

- [@guevararogeljj](https://github.com/guevararogeljj)

## Agradecimientos

- [Dapper](https://github.com/DapperLib/Dapper) - El ORM micro que hace posible este proyecto
- Comunidad .NET por sus herramientas y librerías