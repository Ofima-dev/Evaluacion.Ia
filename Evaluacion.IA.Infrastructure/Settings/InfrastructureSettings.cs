using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Application.Services;
using Evaluacion.IA.Infrastructure.Data;
using Evaluacion.IA.Infrastructure.Repositories;
using Evaluacion.IA.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Evaluacion.IA.Infrastructure.Settings
{
    /// <summary>
    /// Clase de configuración para la capa de Infrastructure
    /// Maneja todas las inyecciones de dependencias y configuraciones necesarias
    /// </summary>
    public static class InfrastructureSettings
    {
        /// <summary>
        /// Configuración de opciones para JWT
        /// </summary>
        public class JwtOptions
        {
            public const string SectionName = "JwtSettings";

            public string SecretKey { get; set; } = string.Empty;
            public string Issuer { get; set; } = string.Empty;
            public string Audience { get; set; } = string.Empty;
            public int ExpirationHours { get; set; } = 24;
        }

        /// <summary>
        /// Configuración de opciones para la base de datos
        /// </summary>
        public class DatabaseOptions
        {
            public const string SectionName = "ConnectionStrings";

            public string DefaultConnection { get; set; } = string.Empty;
        }

        /// <summary>
        /// Configuración de opciones para el hash de contraseñas
        /// </summary>
        public class PasswordHashingOptions
        {
            public const string SectionName = "PasswordHashing";

            public int SaltSize { get; set; } = 16;
            public int HashSize { get; set; } = 32;
            public int Iterations { get; set; } = 4;
            public int MemorySize { get; set; } = 65536; // 64 MB
            public int DegreeOfParallelism { get; set; } = 2;
        }

        /// <summary>
        /// Registra todos los servicios de la capa Infrastructure
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>Colección de servicios actualizada</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuración de opciones
            ConfigureOptions(services, configuration);

            // Configuración de base de datos
            ConfigureDatabase(services, configuration);

            // Configuración de repositorios
            ConfigureRepositories(services);

            // Configuración de servicios
            ConfigureServices(services, configuration);

            return services;
        }

        /// <summary>
        /// Configura las opciones de configuración
        /// </summary>
        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            // Configurar opciones de JWT
            services.Configure<JwtOptions>(
                configuration.GetSection(JwtOptions.SectionName));

            // Configurar opciones de base de datos  
            services.Configure<DatabaseOptions>(
                configuration.GetSection(DatabaseOptions.SectionName));

            // Configurar opciones de hash de contraseñas
            services.Configure<PasswordHashingOptions>(
                configuration.GetSection(PasswordHashingOptions.SectionName));
        }

        /// <summary>
        /// Configura la base de datos y Entity Framework
        /// </summary>
        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Configurar Entity Framework
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Configuraciones específicas de SQL Server
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sqlOptions.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName);
                });

                // Configuraciones adicionales en desarrollo
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });
        }

        /// <summary>
        /// Configura los repositorios y Unit of Work
        /// </summary>
        private static void ConfigureRepositories(IServiceCollection services)
        {
            // Registrar repositorio genérico
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Registrar Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //services.AddScoped<IJWT, JWT>();

            //services.AddScoped<IPasswordHasher, PasswordHasher>();
        }

        /// <summary>
        /// Configura los servicios de la capa Infrastructure
        /// </summary>
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Servicio de hash de contraseñas
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IImageStorageService, ImageStorageService>();
            

            // Servicio JWT
            services.AddScoped<IJWT>(provider =>
            {
                var jwtSettings = configuration.GetSection(JwtOptions.SectionName);
                var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-for-evaluacion-ia-project-development";
                var issuer = jwtSettings["Issuer"] ?? "EvaluacionIA";
                var audience = jwtSettings["Audience"] ?? "EvaluacionIA-Users";

                return new JWT(secretKey, issuer, audience);
            });
        }

        /// <summary>
        /// Ejecuta las migraciones de base de datos al inicializar la aplicación
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <returns>Task</returns>
        public static async Task RunDatabaseMigrationsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                // En una aplicación real, aquí usarías un logger
                throw new InvalidOperationException($"Error applying database migrations: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Seed inicial de datos si es necesario
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <returns>Task</returns>
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            try
            {
                // Verificar si ya existen datos
                if (await context.Roles.AnyAsync())
                    return;

                // Aquí podrías agregar datos iniciales (roles por defecto, etc.)
                // Por ejemplo:
                // var adminRole = new Role(Description.Create("Administrator"));
                // context.Roles.Add(adminRole);
                // await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error seeding database: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validar configuración requerida al inicio
        /// </summary>
        /// <param name="configuration">Configuración</param>
        /// <exception cref="InvalidOperationException">Si falta configuración requerida</exception>
        public static void ValidateConfiguration(IConfiguration configuration)
        {
            // Validar connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is required but was not found in configuration.");
            }

            // Validar configuración JWT
            var jwtSection = configuration.GetSection(JwtOptions.SectionName);
            if (!jwtSection.Exists())
            {
                throw new InvalidOperationException($"Configuration section '{JwtOptions.SectionName}' is required but was not found.");
            }

            // Advertir sobre configuraciones por defecto en producción
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment?.Equals("Production", StringComparison.OrdinalIgnoreCase) == true)
            {
                var secretKey = jwtSection["SecretKey"];
                if (string.IsNullOrEmpty(secretKey) || secretKey.Contains("default"))
                {
                    throw new InvalidOperationException("Production environment requires a proper JWT SecretKey configuration.");
                }
            }
        }
    }
}
