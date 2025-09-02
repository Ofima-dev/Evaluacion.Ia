# Evaluación IA - Clean Architecture .NET 9 API

Un proyecto de API REST desarrollado en .NET 9 que implementa arquitectura limpia (Clean Architecture) con patrones avanzados de diseño. Sistema completo de gestión de productos con categorías, imágenes y usuarios con autenticación segura.

## 🏗️ Arquitectura del Proyecto

Este proyecto sigue los principios de **Clean Architecture** de Robert C. Martin, organizando el código en capas bien definidas con dependencias unidireccionales:

```
┌─────────────────────────────────────────┐
│            API Layer (Controllers)      │
├─────────────────────────────────────────┤
│        Application Layer (Use Cases)    │
├─────────────────────────────────────────┤
│         Domain Layer (Core Business)    │
├─────────────────────────────────────────┤
│        Infrastructure (Data & I/O)      │
└─────────────────────────────────────────┘
```

### Capas del Proyecto

1. **Evaluacion.IA.Domain** - Núcleo del negocio
   - Entidades de dominio
   - Objetos de valor (Value Objects)
   - Primitivos del dominio
   - Lógica de negocio pura

2. **Evaluacion.IA.Application** - Casos de uso
   - Comandos y Consultas (CQRS)
   - Handlers de MediatR
   - DTOs e interfaces
   - Validaciones de aplicación

3. **Evaluacion.IA.Infrastructure** - Implementaciones técnicas
   - Configuraciones de Entity Framework
   - Repositorios y Unit of Work
   - Servicios de seguridad
   - Persistencia de datos

4. **Evaluacion.IA.API** - Capa de presentación
   - Controladores REST
   - Configuración de la aplicación
   - Middleware y filtros

## 🛠️ Tecnologías y Paquetes

### Framework Principal
- **.NET 9.0** - Framework principal con las últimas características

### Base de Datos y ORM
- **Entity Framework Core 9.0.8** - ORM avanzado con configuraciones personalizadas
- **Microsoft.EntityFrameworkCore.SqlServer 9.0.8** - Proveedor SQL Server
- **Microsoft.EntityFrameworkCore.Design 9.0.8** - Herramientas de diseño y migraciones

### Patrones y Mediación
- **MediatR 13.0.0** - Implementación completa del patrón CQRS
- **MediatR.Extensions.Microsoft.DependencyInjection 11.1.0** - Integración con DI

### Seguridad
- **System.IdentityModel.Tokens.Jwt 8.1.4** - Generación y validación de tokens JWT
- **Konscious.Security.Cryptography.Argon2 1.3.1** - Hashing seguro de contraseñas con Argon2id

### Testing
- **xUnit.net 2.9.2** - Framework de pruebas unitarias
- **Microsoft.NET.Test.Sdk 17.12.0** - SDK de pruebas
- **Moq 4.20.72** - Framework de mocking

## 📁 Estructura del Proyecto

```
Evaluacion.Ia/
├── src/
│   ├── Evaluacion.IA.Domain/
│   │   ├── Entities/              # Entidades del dominio
│   │   ├── ValueObjects/          # Objetos de valor
│   │   └── Primitives/           # Tipos primitivos base
│   ├── Evaluacion.IA.Application/
│   │   ├── Commands/             # Comandos CQRS
│   │   ├── Queries/              # Consultas CQRS
│   │   ├── Handlers/             # Manejadores MediatR
│   │   ├── DTOs/                 # Objetos de transferencia
│   │   └── Interfaces/           # Contratos de aplicación
│   ├── Evaluacion.IA.Infrastructure/
│   │   ├── Data/                 # Contexto y configuraciones EF
│   │   ├── Repositories/         # Implementaciones de repositorios
│   │   └── Services/             # Servicios de infraestructura
│   └── Evaluacion.IA.API/
│       ├── Controllers/          # Controladores REST
│       ├── Middleware/           # Middleware personalizado
│       └── Configuration/        # Configuración de la API
└── tests/                        # Proyectos de pruebas
    ├── Evaluacion.IA.Domain.Tests/
    ├── Evaluacion.IA.Application.Tests/
    ├── Evaluacion.IA.Infrastructure.Tests/
    └── Evaluacion.IA.API.Tests/
```

## 🎨 Patrones de Diseño Implementados

### 1. Clean Architecture
- **Separación clara de responsabilidades** por capas
- **Inversión de dependencias** - Las capas internas no conocen las externas
- **Testabilidad** - Cada capa puede probarse independientemente

### 2. CQRS (Command Query Responsibility Segregation)
```csharp
// Comandos para modificar estado
public class CreateUserCommand : IRequest<int>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}

// Consultas para leer datos
public class GetUserByIdQuery : IRequest<UserDto>
{
    public int UserId { get; set; }
}
```

**Implementación:**
- **19 Handlers** en total (11 Commands + 8 Queries)
- Separación completa entre lectura y escritura
- Validaciones específicas por operación

### 3. Repository Pattern con Unit of Work
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Role> Roles { get; }
    IRepository<Category> Categories { get; }
    IRepository<Product> Products { get; }
    IRepository<ProductImage> ProductImages { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### 4. Value Objects Pattern
Implementación completa de objetos de valor para garantizar la integridad del dominio:

```csharp
// Ejemplos de Value Objects implementados
public sealed class Email : ValueObject
public sealed class Money : ValueObject  
public sealed class Name : ValueObject
public sealed class Description : ValueObject
public sealed class Sku : ValueObject
public sealed class Url : ValueObject
```

**Características:**
- **Inmutables** - No pueden modificarse después de la creación
- **Sin identidad** - Se comparan por valor, no por referencia
- **Validación en construcción** - Garantizan datos válidos
- **Conversiones automáticas** en Entity Framework

### 5. Entity Pattern con Domain Primitives
```csharp
public abstract class Entity
{
    public int Id { get; protected set; }
    
    // Implementación de igualdad por identidad
    public override bool Equals(object? obj) { ... }
    public override int GetHashCode() { ... }
}
```

### 6. Domain Services Pattern
Servicios especializados para lógica de dominio compleja:
- **Argon2PasswordHasher** - Hashing seguro de contraseñas
- **JwtTokenGenerator** - Generación de tokens JWT

## 🗄️ Modelo de Dominio

### Entidades Principales

#### 1. User (Usuario)
```csharp
public class User : Entity
{
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public int RoleId { get; private set; }
    public Role? Role { get; private set; }
    public DateTime CreateAt { get; private set; }
}
```

#### 2. Product (Producto)
```csharp
public class Product : Entity
{
    public Sku Sku { get; private set; }
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Money Price { get; private set; }
    public int CategoryId { get; private set; }
    public Category? Category { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<ProductImage> ProductImages { get; }
}
```

#### 3. Category (Categoría)
```csharp
public class Category : Entity
{
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public int? ParentCategoryId { get; private set; }
    public Category? ParentCategory { get; private set; }
    public IReadOnlyCollection<Category> SubCategories { get; }
    public IReadOnlyCollection<Product> Products { get; }
    public bool IsActive { get; private set; }
}
```

### Relaciones del Dominio
- **User ↔ Role**: Relación muchos a uno
- **Product ↔ Category**: Relación muchos a uno con categorías jerárquicas
- **Product ↔ ProductImage**: Relación uno a muchos con imágenes ordenadas
- **Category ↔ SubCategories**: Auto-referencia para jerarquías

## 🔒 Seguridad

### Autenticación JWT
- **Tokens seguros** con firma HMAC SHA256
- **Claims personalizados** para roles y permisos
- **Expiración configurable**

### Hashing de Contraseñas
```csharp
// Implementación con Argon2id
var hasher = new Argon2id(Encoding.UTF8.GetBytes(password))
{
    Salt = salt,
    DegreeOfParallelism = 8,
    MemorySize = 1024 * 1024,
    Iterations = 4
};
```

**Configuración Argon2id:**
- **Algoritmo**: Argon2id (resistente a ataques GPU y side-channel)
- **Memoria**: 1 MB
- **Iteraciones**: 4
- **Paralelismo**: 8 threads

## 🗃️ Persistencia de Datos

### Entity Framework Core Configurations
Configuraciones detalladas para cada entidad:

#### Conversiones de Value Objects
```csharp
builder.Property(p => p.Email)
    .HasConversion(
        email => email.Value,
        value => Email.Create(value));

builder.Property(p => p.Price)
    .HasPrecision(18, 2)
    .HasConversion(
        price => price.Amount,
        value => Money.Create(value, "USD"));
```

#### Restricciones e Índices
- **Índices únicos** en campos críticos (Email de usuario)
- **Restricciones de integridad referencial**
- **Cascadas y restricciones** personalizadas
- **Validaciones a nivel de base de datos**

## 🧪 Testing

### Estructura de Pruebas
- **Domain.Tests** - Pruebas de entidades y value objects
- **Application.Tests** - Pruebas de handlers y lógica de aplicación
- **Infrastructure.Tests** - Pruebas de repositorios y servicios
- **API.Tests** - Pruebas de integración de controladores

### Herramientas de Testing
- **xUnit** para pruebas unitarias
- **Moq** para mocking de dependencias
- **TestContainers** (opcional) para pruebas de integración

## 🚀 Instalación y Uso

### Prerrequisitos
- .NET 9 SDK
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o VS Code

### Configuración

1. **Clonar el repositorio**
```bash
git clone <repository-url>
cd Evaluacion.Ia
```

2. **Restaurar paquetes**
```bash
dotnet restore
```

3. **Configurar cadena de conexión**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=EvaluacionIA;Trusted_Connection=true;"
  }
}
```

4. **Ejecutar migraciones**
```bash
dotnet ef database update -p Evaluacion.IA.Infrastructure -s Evaluacion.IA.API
```

5. **Ejecutar la aplicación**
```bash
dotnet run --project Evaluacion.IA.API
```

### Endpoints Principales

#### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

#### Productos
- `GET /api/products` - Listar productos
- `GET /api/products/{id}` - Obtener producto por ID
- `POST /api/products` - Crear producto
- `PUT /api/products/{id}` - Actualizar producto
- `DELETE /api/products/{id}` - Eliminar producto

#### Categorías
- `GET /api/categories` - Listar categorías
- `GET /api/categories/{id}` - Obtener categoría por ID
- `POST /api/categories` - Crear categoría
- `PUT /api/categories/{id}` - Actualizar categoría

## 📊 Métricas y Características Técnicas

### Estadísticas del Código
- **4 proyectos principales** + 4 de testing
- **5 entidades** de dominio con relaciones complejas
- **6 value objects** implementados
- **19 handlers CQRS** (11 Commands + 8 Queries)
- **5 configuraciones EF** completas con conversiones
- **Cobertura completa** de casos de uso

### Características Avanzadas
- **Transacciones distribuidas** con Unit of Work
- **Validación multinivel** (Domain, Application, Database)
- **Logging estructurado** (opcional con Serilog)
- **Documentación API** con Swagger/OpenAPI
- **Manejo de errores** centralizado
- **Inyección de dependencias** configurada

## 🔮 Patrones Adicionales Identificados

### 1. Specification Pattern (Preparado)
- Estructura lista para implementar especificaciones
- Queries complejas reutilizables

### 2. Domain Events (Preparado)
- Base para implementar eventos de dominio
- Consistencia eventual entre agregados

### 3. Factory Pattern
- Creación controlada de entidades
- Validación en tiempo de construcción

## 🎯 Mejores Prácticas Aplicadas

1. **Immutable Value Objects** - Objetos de valor inmutables
2. **Encapsulación fuerte** - Setters privados en entidades
3. **Fail Fast** - Validaciones tempranas
4. **Separation of Concerns** - Una responsabilidad por clase
5. **Dependency Inversion** - Abstracciones sobre implementaciones
6. **Single Responsibility** - Clases con propósito único
7. **Open/Closed Principle** - Extensible sin modificación

## 📋 Conclusión

Este proyecto demuestra una implementación sofisticada de Clean Architecture en .NET 9, integrando:

- ✅ **Arquitectura robusta** con separación clara de capas
- ✅ **Patrones de diseño** enterprise probados
- ✅ **Seguridad moderna** con JWT y Argon2id
- ✅ **CQRS completo** con MediatR
- ✅ **Domain-Driven Design** con Value Objects
- ✅ **Testing comprehensivo** en todas las capas
- ✅ **Entity Framework** con configuraciones avanzadas
- ✅ **Código mantenible** y extensible

El resultado es una base sólida para aplicaciones enterprise que requieren alta calidad, mantenibilidad y escalabilidad.