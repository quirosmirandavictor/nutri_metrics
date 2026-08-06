using NutriMetrics.Modules.CalorieTracking.Infrastructure;
using NutriMetrics.Modules.Identity.Infrastructure;
using Microsoft.OpenApi;
using NutriMetrics.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// --- DI Container ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var bearerSecurityScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Ingresa tu JWT token (sin 'Bearer ' delante)"
    };

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NutriMetrics API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", bearerSecurityScheme);

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, string.Empty),
            new List<string>()
        }
    });
});

// Modules Registration
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCalorieTrackingModule(builder.Configuration);
// builder.Services.AddAnalyticsModule(builder.Configuration);
builder.Services.AddNutriMetricsObservability(builder.Configuration);
builder.Logging.AddNutriMetricsLogging(builder.Configuration);

var app = builder.Build();

var autoInitializeDatabase = app.Configuration.GetValue("Database:AutoInitialize", true);
if (autoInitializeDatabase)
{
    await app.InitializeDatabaseAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NutriMetrics API v1");
        options.RoutePrefix = "swagger";
    });
    
}

var useHttpsRedirection = app.Configuration.GetValue("Http:UseHttpsRedirection", false);
if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;