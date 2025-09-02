using Evaluacion.IA.Infrastructure.Settings;
using Evaluacion.IA.Application.Settings;
using Evaluacion.IA.API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Validar configuración requerida (todas las capas)
InfrastructureSettings.ValidateConfiguration(builder.Configuration);
ApplicationSettings.ValidateConfiguration(builder.Configuration);
PresentationSettings.ValidateConfiguration(builder.Configuration);

// Agregar servicios de Infrastructure (primero)
builder.Services.AddInfrastructureServices(builder.Configuration);

// Agregar servicios de Application (segundo)
builder.Services.AddApplicationServices(builder.Configuration);

// Agregar servicios de Presentation (tercero)
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

// Configurar pipeline de Presentation
app.ConfigurePresentationPipeline(builder.Configuration);

app.Run();
