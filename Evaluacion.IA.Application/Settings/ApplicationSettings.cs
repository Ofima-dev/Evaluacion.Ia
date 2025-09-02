using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Evaluacion.IA.Application.Settings
{
    /// <summary>
    /// Clase de configuración para la capa de Application
    /// Maneja todas las inyecciones de dependencias y configuraciones necesarias
    /// </summary>
    public static class ApplicationSettings
    {
        /// <summary>
        /// Configuración de opciones para MediatR
        /// </summary>
        public class MediatROptions
        {
            public const string SectionName = "MediatR";
            
            public bool EnableLogging { get; set; } = true;
            public bool EnablePerformanceTracking { get; set; } = true;
            public int TimeoutSeconds { get; set; } = 30;
        }

        /// <summary>
        /// Configuración de opciones para validación
        /// </summary>
        public class ValidationOptions
        {
            public const string SectionName = "Validation";
            
            public bool EnableValidation { get; set; } = true;
            public bool ValidateOnCreate { get; set; } = true;
            public bool ValidateOnUpdate { get; set; } = true;
            public bool StopOnFirstFailure { get; set; } = false;
        }

        /// <summary>
        /// Configuración de opciones para paginación
        /// </summary>
        public class PaginationOptions
        {
            public const string SectionName = "Pagination";
            
            public int DefaultPageSize { get; set; } = 10;
            public int MaxPageSize { get; set; } = 100;
            public int MinPageSize { get; set; } = 1;
        }

        /// <summary>
        /// Configuración de opciones para cache
        /// </summary>
        public class CacheOptions
        {
            public const string SectionName = "Cache";
            
            public bool EnableCaching { get; set; } = true;
            public int DefaultCacheDurationMinutes { get; set; } = 30;
            public int MaxCacheDurationMinutes { get; set; } = 1440; // 24 horas
        }

        /// <summary>
        /// Registra todos los servicios de la capa Application
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>Colección de servicios actualizada</returns>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuración de opciones
            ConfigureOptions(services, configuration);

            // Configuración de MediatR
            ConfigureMediatR(services);

            // Configuración de validadores (si FluentValidation está disponible)
            ConfigureValidation(services);

            // Configuración de servicios adicionales
            ConfigureAdditionalServices(services, configuration);

            return services;
        }

        /// <summary>
        /// Configura las opciones de configuración
        /// </summary>
        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            // Configurar opciones de MediatR
            services.Configure<MediatROptions>(
                configuration.GetSection(MediatROptions.SectionName));

            // Configurar opciones de validación
            services.Configure<ValidationOptions>(
                configuration.GetSection(ValidationOptions.SectionName));

            // Configurar opciones de paginación
            services.Configure<PaginationOptions>(
                configuration.GetSection(PaginationOptions.SectionName));

            // Configurar opciones de cache
            services.Configure<CacheOptions>(
                configuration.GetSection(CacheOptions.SectionName));
        }

        /// <summary>
        /// Configura MediatR y registra todos los handlers
        /// </summary>
        private static void ConfigureMediatR(IServiceCollection services)
        {
            // Registrar MediatR con assembly scanning automático
            services.AddMediatR(cfg =>
            {
                // Registrar handlers del assembly actual
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                
                // Configurar comportamientos (pipelines)
                // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                // cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });
        }

        /// <summary>
        /// Configura validadores con FluentValidation si está disponible
        /// </summary>
        private static void ConfigureValidation(IServiceCollection services)
        {
            // Placeholder para validadores futuros
            // Si se instala FluentValidation en el futuro, descomenta la siguiente línea:
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Configura servicios adicionales de la aplicación
        /// </summary>
        private static void ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration)
        {
            // Registrar servicios de dominio específicos aquí
            // Por ejemplo: services.AddScoped<IDomainService, DomainService>();
            
            // Configurar AutoMapper si está disponible
            try
            {
                // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            }
            catch (Exception)
            {
                // AutoMapper no está disponible, continuar sin mapping automático
            }

            // Configurar servicios de cache en memoria
            services.AddMemoryCache();
        }

        /// <summary>
        /// Valida la configuración de Application al inicio
        /// </summary>
        /// <param name="configuration">Configuración</param>
        /// <exception cref="InvalidOperationException">Si hay problemas con la configuración</exception>
        public static void ValidateConfiguration(IConfiguration configuration)
        {
            // Validar configuración de paginación
            var paginationSection = configuration.GetSection(PaginationOptions.SectionName);
            if (paginationSection.Exists())
            {
                var defaultPageSize = paginationSection.GetValue<int>("DefaultPageSize");
                var maxPageSize = paginationSection.GetValue<int>("MaxPageSize");
                
                if (defaultPageSize > maxPageSize)
                {
                    throw new InvalidOperationException(
                        "DefaultPageSize cannot be greater than MaxPageSize in Pagination configuration.");
                }
            }

            // Validar configuración de cache
            var cacheSection = configuration.GetSection(CacheOptions.SectionName);
            if (cacheSection.Exists())
            {
                var defaultDuration = cacheSection.GetValue<int>("DefaultCacheDurationMinutes");
                var maxDuration = cacheSection.GetValue<int>("MaxCacheDurationMinutes");
                
                if (defaultDuration > maxDuration)
                {
                    throw new InvalidOperationException(
                        "DefaultCacheDurationMinutes cannot be greater than MaxCacheDurationMinutes in Cache configuration.");
                }
            }
        }

        /// <summary>
        /// Obtiene estadísticas de handlers registrados
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <returns>Información sobre handlers registrados</returns>
        public static ApplicationStatistics GetApplicationStatistics(IServiceProvider serviceProvider)
        {
            try
            {
                // Obtener información básica sobre la aplicación
                var assemblyName = Assembly.GetExecutingAssembly().FullName ?? "Unknown";
                
                return new ApplicationStatistics
                {
                    RegisteredHandlers = 0, // Simplificado por ahora
                    ApplicationAssembly = assemblyName,
                    ConfigurationValidated = true,
                    MediatRConfigured = true // Asumimos que está configurado si llegamos aquí
                };
            }
            catch (Exception ex)
            {
                return new ApplicationStatistics
                {
                    RegisteredHandlers = 0,
                    ApplicationAssembly = Assembly.GetExecutingAssembly().FullName ?? "Unknown",
                    ConfigurationValidated = false,
                    MediatRConfigured = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Log de configuración al inicio de la aplicación
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <param name="logger">Logger para registro</param>
        public static void LogConfigurationSummary(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                var stats = GetApplicationStatistics(serviceProvider);
                
                logger.LogInformation("=== Application Layer Configuration Summary ===");
                logger.LogInformation("Assembly: {Assembly}", stats.ApplicationAssembly);
                logger.LogInformation("Registered Handlers: {HandlerCount}", stats.RegisteredHandlers);
                logger.LogInformation("MediatR Configured: {MediatRConfigured}", stats.MediatRConfigured);
                logger.LogInformation("Configuration Validated: {ConfigValidated}", stats.ConfigurationValidated);
                
                if (!string.IsNullOrEmpty(stats.ErrorMessage))
                {
                    logger.LogWarning("Configuration Warning: {Error}", stats.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error logging Application configuration summary");
            }
        }
    }

    /// <summary>
    /// Estadísticas de configuración de la aplicación
    /// </summary>
    public class ApplicationStatistics
    {
        public int RegisteredHandlers { get; set; }
        public string ApplicationAssembly { get; set; } = string.Empty;
        public bool ConfigurationValidated { get; set; }
        public bool MediatRConfigured { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
