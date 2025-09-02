# Application Settings - Configuración de Capa de Aplicación

## Descripción

La carpeta `Settings` contiene la configuración centralizada para la capa de Application del proyecto Evaluación IA. Esta configuración maneja todas las inyecciones de dependencias y configuraciones específicas de la lógica de negocio y casos de uso.

## Archivos

### `ApplicationSettings.cs`
Clase principal que contiene:
- **Registro de MediatR**: Configuración automática de comandos y consultas
- **Configuración de validación**: Preparado para FluentValidation
- **Configuración de paginación**: Parámetros por defecto para listados
- **Configuración de cache**: Cache en memoria para optimización
- **Estadísticas y logging**: Información de configuración

### `appsettings.application.example.json`
Archivo de ejemplo que muestra la estructura de configuración requerida.

## Uso en Program.cs

Para usar esta configuración en tu proyecto API, agrega el siguiente código en `Program.cs`:

```csharp
using Evaluacion.IA.Application.Settings;

var builder = WebApplication.CreateBuilder(args);

// Validar configuración requerida
ApplicationSettings.ValidateConfiguration(builder.Configuration);

// Agregar servicios de Application
builder.Services.AddApplicationServices(builder.Configuration);

// ... otras configuraciones

var app = builder.Build();

// Log de configuración (opcional)
var logger = app.Services.GetRequiredService<ILogger<Program>>();
ApplicationSettings.LogConfigurationSummary(app.Services, logger);

app.Run();
```

## Configuraciones Incluidas

### 🚀 MediatR (CQRS)
- **Registro automático** de todos los handlers
- **Pipeline behaviors** preparados para logging y validación
- **Assembly scanning** automático
- **Configuración de timeouts**

### ✅ Validación
- **Preparado para FluentValidation**
- **Configuración flexible** de validación por operación
- **Manejo de errores** configurable

### 📄 Paginación
- **Parámetros por defecto** configurables
- **Límites de páginas** establecidos
- **Validación automática** de parámetros

### 🚀 Cache
- **Cache en memoria** configurado
- **Duraciones configurables** por tipo de operación
- **Habilitación/deshabilitación** por ambiente

## ⚙️ Opciones de Configuración

### MediatROptions
```json
{
  "MediatR": {
    "EnableLogging": true,
    "EnablePerformanceTracking": true,
    "TimeoutSeconds": 30
  }
}
```

### ValidationOptions
```json
{
  "Validation": {
    "EnableValidation": true,
    "ValidateOnCreate": true,
    "ValidateOnUpdate": true,
    "StopOnFirstFailure": false
  }
}
```

### PaginationOptions
```json
{
  "Pagination": {
    "DefaultPageSize": 10,
    "MaxPageSize": 100,
    "MinPageSize": 1
  }
}
```

### CacheOptions
```json
{
  "Cache": {
    "EnableCaching": true,
    "DefaultCacheDurationMinutes": 30,
    "MaxCacheDurationMinutes": 1440
  }
}
```

## 🛠️ Servicios Registrados

### MediatR y CQRS
- Todos los **Command Handlers** registrados automáticamente
- Todos los **Query Handlers** registrados automáticamente
- **Pipeline behaviors** configurados (logging, validación, performance)

### Cache
- `IMemoryCache` registrado y configurado

### Configuración
- Todas las opciones strongly-typed disponibles via DI

## 📋 Métodos de Extensión

### `AddApplicationServices()`
Registra todos los servicios de Application en una sola llamada:
```csharp
builder.Services.AddApplicationServices(builder.Configuration);
```

### `ValidateConfiguration()`
Valida configuraciones específicas de Application:
```csharp
ApplicationSettings.ValidateConfiguration(builder.Configuration);
```

### `GetApplicationStatistics()`
Obtiene estadísticas de la configuración:
```csharp
var stats = ApplicationSettings.GetApplicationStatistics(app.Services);
```

### `LogConfigurationSummary()`
Registra un resumen de la configuración:
```csharp
ApplicationSettings.LogConfigurationSummary(app.Services, logger);
```

## 🔍 Validaciones Implementadas

### **Configuraciones de Paginación:**
- ✅ DefaultPageSize ≤ MaxPageSize
- ✅ Valores numéricos válidos

### **Configuraciones de Cache:**
- ✅ DefaultCacheDurationMinutes ≤ MaxCacheDurationMinutes
- ✅ Duraciones positivas

## 🎨 Características Avanzadas

### **Registro Automático:**
- Assembly scanning para handlers de MediatR
- Configuración automática de pipeline behaviors
- Preparado para validadores futuros

### **Estadísticas:**
- Información sobre handlers registrados
- Estado de configuración
- Información de assembly y versión

### **Extensibilidad:**
- Preparado para AutoMapper
- Preparado para FluentValidation
- Fácil adición de nuevos servicios

## 🚀 Integración con Infrastructure

Se integra perfectamente con InfrastructureSettings:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Validar configuración
InfrastructureSettings.ValidateConfiguration(builder.Configuration);
ApplicationSettings.ValidateConfiguration(builder.Configuration);

// 2. Registrar servicios (orden importante)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

// 3. Servicios de API
builder.Services.AddControllers();

var app = builder.Build();

// 4. Inicialización
await InfrastructureSettings.RunDatabaseMigrationsAsync(app.Services);
var logger = app.Services.GetRequiredService<ILogger<Program>>();
ApplicationSettings.LogConfigurationSummary(app.Services, logger);

app.Run();
```

## 📦 Handlers Registrados Automáticamente

La configuración registra automáticamente todos los handlers encontrados:

### **Auth Handlers:**
- `LoginCommandHandler`

### **User Handlers:**
- `GetAllUsersQueryHandler`
- `GetUserByIdQueryHandler`
- `CreateUserCommandHandler`
- `UpdateUserCommandHandler`
- `DeleteUserCommandHandler`
- `ChangePasswordCommandHandler`

### **Role Handlers:**
- `GetAllRolesQueryHandler`
- `GetRoleByIdQueryHandler`
- `GetAvailableRolesQueryHandler`
- `CreateRoleCommandHandler`
- `UpdateRoleCommandHandler`
- `DeleteRoleCommandHandler`

### **Product Handlers:**
- `GetAllProductsQueryHandler`
- `GetProductByIdQueryHandler`
- `GetProductsByCategoryQueryHandler`
- `SearchProductsQueryHandler`
- `CreateProductCommandHandler`
- `UpdateProductCommandHandler`
- `DeleteProductCommandHandler`

### **Category Handlers:**
- `GetAllCategoriesQueryHandler`
- `GetCategoryByIdQueryHandler`
- `GetActiveCategoriesQueryHandler`
- `CreateCategoryCommandHandler`
- `UpdateCategoryCommandHandler`
- `DeleteCategoryCommandHandler`

### **Product Image Handlers:**
- `GetProductImagesQueryHandler`
- `AddProductImageCommandHandler`
- `UpdateProductImageCommandHandler`
- `DeleteProductImageCommandHandler`
- `SetPrimaryImageCommandHandler`
- `ReorderProductImagesCommandHandler`

## 📚 Dependencias NuGet

Las siguientes dependencias se incluyen automáticamente:
- `MediatR v13.0.0`
- `Microsoft.Extensions.Options.ConfigurationExtensions v9.0.8`
- `Microsoft.Extensions.Caching.Memory v9.0.8`
- `Microsoft.Extensions.Logging.Abstractions v9.0.8`

## ✅ Estado de Compilación

- ✅ **Sin errores** de compilación
- ✅ **Todos los handlers** se registran automáticamente
- ✅ **Configuración flexible** y extensible
- ✅ **Preparado para futuras extensiones**

## 🎉 Beneficios Logrados

### **Para el Desarrollador:**
- 🔧 **Una sola línea** para configurar toda la Application
- 🚀 **Registro automático** de todos los handlers
- 📖 **Documentación clara** de configuraciones

### **Para la Aplicación:**
- 🏗️ **CQRS completamente configurado**
- 🔒 **Validaciones preparadas**
- 🚀 **Performance optimizado** con cache
- 📊 **Estadísticas de configuración**

### **Para el Proyecto:**
- 📋 **Separación de responsabilidades** entre capas
- 🔄 **Configuración centralizada** y reutilizable
- 🧪 **Testeable** y modular
- 📈 **Escalable** para nuevos casos de uso

---

**La configuración de Application está completa y funciona en conjunto con Infrastructure para proporcionar una solución completa.** ✅
