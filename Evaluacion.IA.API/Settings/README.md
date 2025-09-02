# Presentation Settings - Configuración de Capa de Presentación (API)

## Descripción

La carpeta `Settings` contiene la configuración centralizada para la capa de Presentación (API) del proyecto Evaluación IA. Esta configuración maneja todas las inyecciones de dependencias y configuraciones específicas de la API web, incluyendo autenticación, autorización, CORS, Swagger, y más.

## Archivos

### `PresentationSettings.cs`
Clase principal que contiene:
- **Configuración de autenticación JWT**: Bearer tokens y validación
- **Configuración de autorización**: Políticas por roles y claims
- **Configuración de CORS**: Políticas de origen cruzado
- **Configuración de Swagger**: Documentación automática de API
- **Configuración de controladores**: Serialización JSON y validación
- **Configuración del pipeline**: Middleware y manejo de errores

### `appsettings.presentation.example.json`
Archivo de ejemplo que muestra la estructura de configuración requerida.

## Uso en Program.cs

Para usar esta configuración en tu proyecto API, agrega el siguiente código en `Program.cs`:

```csharp
using Evaluacion.IA.API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Validar configuración requerida
PresentationSettings.ValidateConfiguration(builder.Configuration);

// Agregar servicios de Presentation
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

// Configurar pipeline de presentation
app.ConfigurePresentationPipeline(builder.Configuration);

app.Run();
```

## Configuraciones Incluidas

### 🔐 Autenticación JWT
- **Bearer Token** authentication configurado
- **Validación completa** de tokens (issuer, audience, lifetime)
- **Configuración flexible** de parámetros de validación
- **Manejo de eventos** de autenticación

### 🛡️ Autorización
- **Políticas por defecto** y personalizadas
- **Autorización basada en roles**: Admin, User
- **Autorización basada en claims**: Permisos específicos
- **Políticas combinadas**: AdminOrUser

### 🌐 CORS (Cross-Origin Resource Sharing)
- **Orígenes específicos** configurables
- **Métodos HTTP** permitidos
- **Headers personalizados** 
- **Credenciales** habilitadas/deshabilitadas

### 📚 Swagger/OpenAPI
- **Documentación automática** de endpoints
- **Integración JWT** para testing
- **Comentarios XML** incluidos
- **Configuración personalizable** (título, versión, contacto)

### 🎛️ Controladores
- **Serialización JSON** optimizada (camelCase)
- **Validación automática** de modelos
- **Filtros globales** de validación
- **Configuración de rutas** flexible

### ⚡ Características Avanzadas
- **Rate Limiting** preparado (para implementación futura)
- **Pipeline de middleware** optimizado
- **Manejo de errores** por ambiente
- **Estadísticas** de configuración

## ⚙️ Opciones de Configuración

### JwtAuthenticationOptions
```json
{
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-segura",
    "Issuer": "EvaluacionIA",
    "Audience": "EvaluacionIA-Users",
    "ExpirationHours": 24,
    "ValidateIssuerSigningKey": true,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true
  }
}
```

### CorsOptions
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://yourdomain.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowCredentials": true,
    "PolicyName": "DefaultCorsPolicy"
  }
}
```

### SwaggerOptions
```json
{
  "Swagger": {
    "Enabled": true,
    "Title": "Evaluación IA API",
    "Version": "v1",
    "Description": "API para el sistema de evaluación con IA",
    "IncludeXmlComments": true,
    "EnableJwtAuthentication": true
  }
}
```

### ControllerOptions
```json
{
  "Controllers": {
    "EnableModelValidation": true,
    "SuppressAsyncSuffixInActionNames": true,
    "DefaultRoute": "api/[controller]"
  }
}
```

## 🛠️ Servicios Registrados

### Autenticación y Autorización
- `JwtBearer` authentication scheme configurado
- Políticas de autorización personalizadas
- Token validation parameters

### API y Controladores
- Controladores con configuración JSON optimizada
- Endpoint API explorer para Swagger
- Filtros de validación global

### Documentación
- Swagger/OpenAPI generator configurado
- XML comments integration
- JWT authentication in Swagger UI

### CORS
- Política CORS configurada con orígenes específicos
- Soporte para credenciales si es requerido

## 📋 Métodos de Extensión

### `AddPresentationServices()`
Registra todos los servicios de Presentation en una sola llamada:
```csharp
builder.Services.AddPresentationServices(builder.Configuration);
```

### `ConfigurePresentationPipeline()`
Configura todo el pipeline de middleware:
```csharp
app.ConfigurePresentationPipeline(builder.Configuration);
```

### `ValidateConfiguration()`
Valida configuraciones críticas de seguridad:
```csharp
PresentationSettings.ValidateConfiguration(builder.Configuration);
```

### `GetPresentationStatistics()`
Obtiene estadísticas de la configuración:
```csharp
var stats = PresentationSettings.GetPresentationStatistics(app.Services);
```

## 🔍 Validaciones Implementadas

### **Seguridad JWT:**
- ✅ Clave secreta segura en producción (mínimo 32 caracteres)
- ✅ No usar claves por defecto en producción
- ✅ Configuración de validación completa

### **CORS:**
- ✅ Orígenes específicos cuando se permiten credenciales
- ✅ Configuración segura para producción

## 🚀 Pipeline de Middleware

El pipeline se configura automáticamente en el siguiente orden:

```csharp
// Desarrollo
app.UseDeveloperExceptionPage(); // Solo en desarrollo
app.UseSwagger();                // Solo en desarrollo
app.UseSwaggerUI();              // Solo en desarrollo

// Producción
app.UseExceptionHandler("/Error"); // Solo en producción
app.UseHsts();                     // Solo en producción

// Común
app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

## 🎨 Políticas de Autorización

### Políticas por Rol:
- **`AdminOnly`**: Solo administradores
- **`UserOnly`**: Solo usuarios regulares  
- **`AdminOrUser`**: Administradores o usuarios

### Políticas por Claims:
- **`CanManageUsers`**: Permiso para gestionar usuarios
- **`CanManageProducts`**: Permiso para gestionar productos

### Uso en Controladores:
```csharp
[Authorize(Policy = "AdminOnly")]
public class UsersController : ControllerBase { }

[Authorize(Policy = "CanManageProducts")]
[HttpPost]
public async Task<IActionResult> CreateProduct() { }
```

## 📊 Swagger/OpenAPI Features

### Características Incluidas:
- ✅ **Documentación automática** de todos los endpoints
- ✅ **Integración JWT** para testing directo
- ✅ **Comentarios XML** de controllers y actions
- ✅ **Información de contacto** personalizable
- ✅ **Versionado** de API
- ✅ **Esquemas de seguridad** configurados

### Acceso:
- **Swagger JSON**: `/swagger/v1/swagger.json`
- **Swagger UI**: `/swagger`

## 🔧 Integración Completa

Se integra perfectamente con las otras capas:

```csharp
var builder = WebApplication.CreateBuilder(args);

// 1. Validar configuración de todas las capas
InfrastructureSettings.ValidateConfiguration(builder.Configuration);
ApplicationSettings.ValidateConfiguration(builder.Configuration);
PresentationSettings.ValidateConfiguration(builder.Configuration);

// 2. Registrar servicios (orden importante)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

// 3. Configurar pipeline
app.ConfigurePresentationPipeline(builder.Configuration);

// 4. Inicialización
await InfrastructureSettings.RunDatabaseMigrationsAsync(app.Services);

app.Run();
```

## 📦 Paquetes NuGet Agregados

Las siguientes dependencias se incluyen automáticamente:
- `Microsoft.AspNetCore.Authentication.JwtBearer v9.0.8`
- `Swashbuckle.AspNetCore v9.0.4`
- `Microsoft.AspNetCore.OpenApi v9.0.8` (ya incluido)

## ⚡ Características de Performance

### JSON Serialization:
- **CamelCase** naming policy
- **Null value ignoring** para respuestas más pequeñas
- **Configuración optimizada** para APIs

### Middleware Pipeline:
- **HTTPS redirection** automático
- **CORS** optimizado
- **Authentication/Authorization** eficiente

## 🛡️ Seguridad

### Mejores Prácticas Implementadas:
- ✅ **HTTPS** forzado en producción
- ✅ **JWT** con validación completa
- ✅ **CORS** restrictivo en producción
- ✅ **HSTS** habilitado en producción
- ✅ **Validación automática** de modelos

### Configuración por Ambiente:
- **Development**: Errores detallados, Swagger habilitado
- **Production**: Manejo de errores seguro, HSTS habilitado

## ✅ Estado de Compilación

- ✅ **Sin errores** de compilación
- ✅ **Integración completa** con Infrastructure y Application
- ✅ **Pipeline optimizado** para desarrollo y producción
- ✅ **Documentación automática** funcionando

## 🎉 Beneficios Logrados

### **Para el Desarrollador:**
- 🔧 **Una línea** configura toda la presentación
- 🔍 **Swagger integrado** para testing inmediato
- 📖 **Documentación automática** de endpoints
- 🚀 **Pipeline completo** preconfigurado

### **Para la API:**
- 🔒 **Seguridad completa** JWT + CORS + HTTPS
- 📊 **Documentación automática** profesional
- ⚡ **Performance optimizado** con JSON eficiente
- 🛡️ **Validación automática** de requests

### **Para el Proyecto:**
- 📋 **Configuración centralizada** y mantenible
- 🔄 **Pipeline consistente** entre ambientes
- 🧪 **Fácil testing** con Swagger UI
- 📈 **Escalable** para nuevas características

---

**La configuración de Presentation está completa y proporciona una API web profesional, segura y bien documentada.** ✅
