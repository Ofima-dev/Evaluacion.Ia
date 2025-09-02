using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Evaluacion.IA.API.Settings
{
    /// <summary>
    /// Clase de configuración para la capa de Presentación (API)
    /// Maneja todas las inyecciones de dependencias y configuraciones específicas de la API web
    /// </summary>
    public static class PresentationSettings
    {
        /// <summary>
        /// Configuración de opciones para autenticación JWT
        /// </summary>
        public class JwtAuthenticationOptions
        {
            public const string SectionName = "JwtSettings";
            
            public string SecretKey { get; set; } = string.Empty;
            public string Issuer { get; set; } = string.Empty;
            public string Audience { get; set; } = string.Empty;
            public int ExpirationHours { get; set; } = 24;
            public bool ValidateIssuerSigningKey { get; set; } = true;
            public bool ValidateIssuer { get; set; } = true;
            public bool ValidateAudience { get; set; } = true;
            public bool ValidateLifetime { get; set; } = true;
        }

        /// <summary>
        /// Configuración de opciones para CORS
        /// </summary>
        public class CorsOptions
        {
            public const string SectionName = "Cors";
            
            public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
            public string[] AllowedMethods { get; set; } = { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
            public string[] AllowedHeaders { get; set; } = { "*" };
            public bool AllowCredentials { get; set; } = true;
            public string PolicyName { get; set; } = "DefaultCorsPolicy";
        }

        /// <summary>
        /// Configuración de opciones para Swagger/OpenAPI
        /// </summary>
        public class SwaggerOptions
        {
            public const string SectionName = "Swagger";
            
            public bool Enabled { get; set; } = true;
            public string Title { get; set; } = "Evaluación IA API";
            public string Version { get; set; } = "v1";
            public string Description { get; set; } = "API para el sistema de evaluación con IA";
            public string ContactName { get; set; } = "Equipo de Desarrollo";
            public string ContactEmail { get; set; } = "dev@evaluacionia.com";
            public bool IncludeXmlComments { get; set; } = true;
            public bool EnableJwtAuthentication { get; set; } = true;
        }

        /// <summary>
        /// Configuración de opciones para controladores
        /// </summary>
        public class ControllerOptions
        {
            public const string SectionName = "Controllers";
            
            public bool EnableModelValidation { get; set; } = true;
            public bool SuppressAsyncSuffixInActionNames { get; set; } = true;
            public bool EnableEndpointRouting { get; set; } = true;
            public string DefaultRoute { get; set; } = "api/[controller]";
        }

        /// <summary>
        /// Configuración de opciones para serialización JSON
        /// </summary>
        public class JsonOptions
        {
            public const string SectionName = "Json";
            
            public bool PropertyNameCaseInsensitive { get; set; } = true;
            public JsonNamingPolicy PropertyNamingPolicy { get; set; } = JsonNamingPolicy.CamelCase;
            public bool IncludeFields { get; set; } = true;
            public bool WriteIndented { get; set; } = false;
            public JsonIgnoreCondition DefaultIgnoreCondition { get; set; } = JsonIgnoreCondition.WhenWritingNull;
        }

        /// <summary>
        /// Configuración de opciones para rate limiting
        /// </summary>
        public class RateLimitOptions
        {
            public const string SectionName = "RateLimit";
            
            public bool Enabled { get; set; } = true;
            public int RequestsPerMinute { get; set; } = 60;
            public int RequestsPerHour { get; set; } = 1000;
            public string[] ExemptEndpoints { get; set; } = Array.Empty<string>();
        }

        /// <summary>
        /// Registra todos los servicios de la capa de Presentación
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>Colección de servicios actualizada</returns>
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configuración de opciones
            ConfigureOptions(services, configuration);

            // Configuración de controladores
            ConfigureControllers(services, configuration);

            // Configuración de autenticación JWT
            ConfigureJwtAuthentication(services, configuration);

            // Configuración de autorización
            ConfigureAuthorization(services);

            // Configuración de CORS
            ConfigureCors(services, configuration);

            // Configuración de Swagger/OpenAPI
            ConfigureSwagger(services, configuration);

            // Configuración de rate limiting
            ConfigureRateLimit(services, configuration);

            return services;
        }

        /// <summary>
        /// Configura las opciones de configuración
        /// </summary>
        private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtAuthenticationOptions>(
                configuration.GetSection(JwtAuthenticationOptions.SectionName));

            services.Configure<CorsOptions>(
                configuration.GetSection(CorsOptions.SectionName));

            services.Configure<SwaggerOptions>(
                configuration.GetSection(SwaggerOptions.SectionName));

            services.Configure<ControllerOptions>(
                configuration.GetSection(ControllerOptions.SectionName));

            services.Configure<JsonOptions>(
                configuration.GetSection(JsonOptions.SectionName));

            services.Configure<RateLimitOptions>(
                configuration.GetSection(RateLimitOptions.SectionName));
        }

        /// <summary>
        /// Configura los controladores y APIs
        /// </summary>
        private static void ConfigureControllers(IServiceCollection services, IConfiguration configuration)
        {
            var jsonOptions = configuration.GetSection(JsonOptions.SectionName);
            
            services.AddControllers(options =>
            {
                // Configuración de model binding
                var controllerConfig = configuration.GetSection(ControllerOptions.SectionName);
                if (controllerConfig.GetValue<bool>("SuppressAsyncSuffixInActionNames"))
                {
                    options.SuppressAsyncSuffixInActionNames = true;
                }
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Configuración de validación automática
                options.SuppressModelStateInvalidFilter = false;
                options.SuppressMapClientErrors = false;
            })
            .AddJsonOptions(options =>
            {
                // Configuración de serialización JSON
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = 
                    jsonOptions.GetValue<bool>("PropertyNameCaseInsensitive", true);
                
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = 
                    jsonOptions.GetValue<bool>("WriteIndented", false);
                
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            // API Explorer para Swagger
            services.AddEndpointsApiExplorer();
        }

        /// <summary>
        /// Configura la autenticación JWT
        /// </summary>
        private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(JwtAuthenticationOptions.SectionName);
            var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-for-evaluacion-ia-project-development";
            var issuer = jwtSettings["Issuer"] ?? "EvaluacionIA";
            var audience = jwtSettings["Audience"] ?? "EvaluacionIA-Users";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = jwtSettings.GetValue<bool>("ValidateIssuerSigningKey", true),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ValidateIssuer = jwtSettings.GetValue<bool>("ValidateIssuer", true),
                        ValidIssuer = issuer,
                        ValidateAudience = jwtSettings.GetValue<bool>("ValidateAudience", true),
                        ValidAudience = audience,
                        ValidateLifetime = jwtSettings.GetValue<bool>("ValidateLifetime", true),
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            // Log authentication failures
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            // Log successful token validation
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        /// <summary>
        /// Configura las políticas de autorización
        /// </summary>
        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Política por defecto
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Políticas específicas por rol
                options.AddPolicy("AdminOnly", policy => 
                    policy.RequireRole("Administrator"));

                options.AddPolicy("UserOnly", policy => 
                    policy.RequireRole("User"));

                options.AddPolicy("AdminOrUser", policy => 
                    policy.RequireRole("Administrator", "User"));

                // Políticas basadas en claims
                options.AddPolicy("CanManageUsers", policy => 
                    policy.RequireClaim("Permission", "ManageUsers"));

                options.AddPolicy("CanManageProducts", policy => 
                    policy.RequireClaim("Permission", "ManageProducts"));
            });
        }

        /// <summary>
        /// Configura CORS
        /// </summary>
        private static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
        {
            var corsSettings = configuration.GetSection(CorsOptions.SectionName);
            var policyName = corsSettings.GetValue<string>("PolicyName", "DefaultCorsPolicy");
            
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>();
                    
                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        builder.WithOrigins(allowedOrigins);
                    }
                    else
                    {
                        builder.AllowAnyOrigin();
                    }

                    builder.AllowAnyMethod()
                           .AllowAnyHeader();

                    if (corsSettings.GetValue<bool>("AllowCredentials", true) && 
                        (allowedOrigins != null && allowedOrigins.Length > 0))
                    {
                        builder.AllowCredentials();
                    }
                });
            });
        }

        /// <summary>
        /// Configura Swagger/OpenAPI
        /// </summary>
        private static void ConfigureSwagger(IServiceCollection services, IConfiguration configuration)
        {
            var swaggerSettings = configuration.GetSection(SwaggerOptions.SectionName);
            
            if (!swaggerSettings.GetValue<bool>("Enabled", true))
                return;

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = swaggerSettings.GetValue<string>("Title", "Evaluación IA API"),
                    Version = swaggerSettings.GetValue<string>("Version", "v1"),
                    Description = swaggerSettings.GetValue<string>("Description", "API para el sistema de evaluación con IA"),
                    Contact = new OpenApiContact
                    {
                        Name = swaggerSettings.GetValue<string>("ContactName", "Equipo de Desarrollo"),
                        Email = swaggerSettings.GetValue<string>("ContactEmail", "dev@evaluacionia.com")
                    }
                });

                // Configuración JWT en Swagger
                if (swaggerSettings.GetValue<bool>("EnableJwtAuthentication", true))
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                }

                // Incluir comentarios XML
                if (swaggerSettings.GetValue<bool>("IncludeXmlComments", true))
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    
                    if (File.Exists(xmlPath))
                    {
                        options.IncludeXmlComments(xmlPath);
                    }
                }
            });
        }

        /// <summary>
        /// Configura rate limiting (placeholder para futuro)
        /// </summary>
        private static void ConfigureRateLimit(IServiceCollection services, IConfiguration configuration)
        {
            var rateLimitSettings = configuration.GetSection(RateLimitOptions.SectionName);
            
            if (!rateLimitSettings.GetValue<bool>("Enabled", false))
                return;

            // Placeholder para rate limiting
            // En el futuro se puede implementar con AspNetCoreRateLimit o similar
            // services.AddMemoryCache();
            // services.Configure<IpRateLimitOptions>(rateLimitSettings);
        }

        /// <summary>
        /// Configura el pipeline de middleware
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="configuration">Configuración</param>
        /// <returns>Application builder actualizado</returns>
        public static WebApplication ConfigurePresentationPipeline(
            this WebApplication app, 
            IConfiguration configuration)
        {
            var environment = app.Environment;

            // Middleware de desarrollo
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ConfigureSwaggerPipeline(app, configuration);
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Middleware de seguridad
            app.UseHttpsRedirection();

            // CORS
            var corsSettings = configuration.GetSection(CorsOptions.SectionName);
            var policyName = corsSettings.GetValue<string>("PolicyName", "DefaultCorsPolicy");
            app.UseCors(policyName);

            // Autenticación y autorización
            app.UseAuthentication();
            app.UseAuthorization();

            // Controladores
            app.MapControllers();

            return app;
        }

        /// <summary>
        /// Configura el pipeline de Swagger
        /// </summary>
        private static void ConfigureSwaggerPipeline(WebApplication app, IConfiguration configuration)
        {
            var swaggerSettings = configuration.GetSection(SwaggerOptions.SectionName);
            
            if (swaggerSettings.GetValue<bool>("Enabled", true))
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", 
                        swaggerSettings.GetValue<string>("Title", "Evaluación IA API"));
                    options.RoutePrefix = "swagger";
                    options.DisplayRequestDuration();
                });
            }
        }

        /// <summary>
        /// Valida la configuración de la capa de presentación
        /// </summary>
        /// <param name="configuration">Configuración</param>
        /// <exception cref="InvalidOperationException">Si hay problemas con la configuración</exception>
        public static void ValidateConfiguration(IConfiguration configuration)
        {
            // Validar configuración JWT
            var jwtSection = configuration.GetSection(JwtAuthenticationOptions.SectionName);
            if (jwtSection.Exists())
            {
                var secretKey = jwtSection["SecretKey"];
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                
                if (environment?.Equals("Production", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (string.IsNullOrEmpty(secretKey) || secretKey.Contains("default"))
                    {
                        throw new InvalidOperationException(
                            "Production environment requires a secure JWT SecretKey configuration.");
                    }

                    if (secretKey.Length < 32)
                    {
                        throw new InvalidOperationException(
                            "JWT SecretKey must be at least 32 characters long in production.");
                    }
                }
            }

            // Validar configuración CORS
            var corsSection = configuration.GetSection(CorsOptions.SectionName);
            if (corsSection.Exists())
            {
                var allowCredentials = corsSection.GetValue<bool>("AllowCredentials", false);
                var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>();
                
                if (allowCredentials && (allowedOrigins == null || allowedOrigins.Length == 0))
                {
                    throw new InvalidOperationException(
                        "When AllowCredentials is true, specific origins must be configured for CORS.");
                }
            }
        }

        /// <summary>
        /// Obtiene estadísticas de la configuración de presentación
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <returns>Estadísticas de configuración</returns>
        public static PresentationStatistics GetPresentationStatistics(IServiceProvider serviceProvider)
        {
            try
            {
                return new PresentationStatistics
                {
                    ControllersRegistered = GetControllerCount(serviceProvider),
                    JwtConfigured = serviceProvider.GetService<IConfiguration>()
                        ?.GetSection(JwtAuthenticationOptions.SectionName).Exists() ?? false,
                    SwaggerEnabled = serviceProvider.GetService<IConfiguration>()
                        ?.GetSection(SwaggerOptions.SectionName).GetValue<bool>("Enabled", true) ?? false,
                    CorsConfigured = serviceProvider.GetService<IConfiguration>()
                        ?.GetSection(CorsOptions.SectionName).Exists() ?? false,
                    AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Unknown"
                };
            }
            catch (Exception ex)
            {
                return new PresentationStatistics
                {
                    ErrorMessage = ex.Message,
                    AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "Unknown"
                };
            }
        }

        /// <summary>
        /// Obtiene el número de controladores registrados
        /// </summary>
        private static int GetControllerCount(IServiceProvider serviceProvider)
        {
            try
            {
                var controllerTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && !t.IsAbstract)
                    .ToArray();
                
                return controllerTypes.Length;
            }
            catch
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Filtro de validación global
    /// </summary>
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
            var errors = context.ModelState
                .Where(x => x.Value != null)
                .SelectMany(x => x.Value!.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                .ToArray();                var response = new
                {
                    Message = "Validation failed",
                    Errors = errors
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }

    /// <summary>
    /// Estadísticas de configuración de presentación
    /// </summary>
    public class PresentationStatistics
    {
        public int ControllersRegistered { get; set; }
        public bool JwtConfigured { get; set; }
        public bool SwaggerEnabled { get; set; }
        public bool CorsConfigured { get; set; }
        public string AssemblyName { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
