# Infrastructure Settings - Configuración de Capa de Infraestructura

## Descripción

La carpeta `Settings` contiene la configuración centralizada para la capa de Infrastructure del proyecto Evaluación IA. Esta configuración maneja todas las inyecciones de dependencias y configuraciones necesarias para el funcionamiento de la capa de infraestructura.

## Archivos

### `InfrastructureSettings.cs`
Clase principal que contiene:
- **Registro de servicios**: Configuración de inyección de dependencias
- **Configuración de base de datos**: Entity Framework y SQL Server
- **Configuración de seguridad**: JWT y hash de contraseñas
- **Validación de configuración**: Verificación de configuraciones requeridas

### `appsettings.example.json`
Archivo de ejemplo que muestra la estructura de configuración requerida.

## Uso en Program.cs

Para usar esta configuración en tu proyecto API, agrega el siguiente código en `Program.cs`:

```csharp
using Evaluacion.IA.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Validar configuración requerida
InfrastructureSettings.ValidateConfiguration(builder.Configuration);

// Agregar servicios de Infrastructure
builder.Services.AddInfrastructureServices(builder.Configuration);

// ... otras configuraciones

var app = builder.Build();

// Ejecutar migraciones al inicio (opcional)
await InfrastructureSettings.RunDatabaseMigrationsAsync(app.Services);

// Seed de datos iniciales (opcional)
await InfrastructureSettings.SeedDatabaseAsync(app.Services);

app.Run();
```

## Configuraciones Incluidas

### 🗄️ Base de Datos
- **Entity Framework Core** con SQL Server
- **Configuración de reintentos** automáticos
- **Migraciones automáticas**
- **Logging detallado** en desarrollo

### 🔐 Seguridad
- **Servicio JWT** para autenticación
- **Hash de contraseñas** con Argon2
- **Configuración de secretos**

### 📦 Repositorios
- **Patrón Repository** genérico
- **Unit of Work** para transacciones
- **Inyección automática** de repositorios

### ⚙️ Opciones de Configuración

#### JwtSettings
```json
{
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-aquí",
    "Issuer": "EvaluacionIA", 
    "Audience": "EvaluacionIA-Users",
    "ExpirationHours": 24
  }
}
```

#### ConnectionStrings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=EvaluacionIA;..."
  }
}
```

#### PasswordHashing
```json
{
  "PasswordHashing": {
    "SaltSize": 16,
    "HashSize": 32,
    "Iterations": 4,
    "MemorySize": 65536,
    "DegreeOfParallelism": 2
  }
}
```

## Servicios Registrados

### Interfaces y Implementaciones
- `IRepository<T>` → `Repository<T>`
- `IUnitOfWork` → `UnitOfWork`
- `IPasswordHasher` → `PasswordHasher`
- `IJWT` → `JWT`

### Contextos
- `DatabaseContext` con configuración de SQL Server

## Validaciones

El sistema incluye validaciones automáticas para:
- ✅ Connection string requerido
- ✅ Configuración JWT presente
- ✅ Secretos seguros en producción
- ✅ Variables de entorno apropiadas

## Características Adicionales

### 🔄 Migraciones Automáticas
```csharp
await InfrastructureSettings.RunDatabaseMigrationsAsync(app.Services);
```

### 🌱 Seed de Datos
```csharp
await InfrastructureSettings.SeedDatabaseAsync(app.Services);
```

### 🛠️ Configuración por Ambiente
- Configuración específica para **Development**
- Validaciones estrictas para **Production**
- Logging detallado configurable

## Ejemplo de Uso Completo

```csharp
// Program.cs
using Evaluacion.IA.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

try
{
    // 1. Validar configuración
    InfrastructureSettings.ValidateConfiguration(builder.Configuration);
    
    // 2. Registrar servicios de Infrastructure
    builder.Services.AddInfrastructureServices(builder.Configuration);
    
    // 3. Otros servicios (Application, API, etc.)
    builder.Services.AddControllers();
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    
    var app = builder.Build();
    
    // 4. Configurar pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    
    // 5. Inicialización de base de datos
    await InfrastructureSettings.RunDatabaseMigrationsAsync(app.Services);
    await InfrastructureSettings.SeedDatabaseAsync(app.Services);
    
    app.Run();
}
catch (Exception ex)
{
    // Log error y salir
    Console.WriteLine($"Error during application startup: {ex.Message}");
    throw;
}
```

## Dependencias NuGet

Las siguientes dependencias se incluyen automáticamente:
- `Microsoft.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`  
- `Microsoft.Extensions.Options.ConfigurationExtensions`
- `System.IdentityModel.Tokens.Jwt`
- `Konscious.Security.Cryptography.Argon2`

## Notas de Seguridad

⚠️ **Importante para Producción:**
- Cambiar la clave JWT por una segura
- Usar connection strings cifrados
- Configurar variables de entorno apropiadas
- Habilitar HTTPS
- Configurar CORS adecuadamente
