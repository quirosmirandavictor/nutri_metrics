using NutriMetrics.Modules.CalorieTracking.Infrastructure;
using NutriMetrics.Modules.Identity.Infrastructure;
using Microsoft.OpenApi; 

var builder = WebApplication.CreateBuilder(args);

// --- DI Container ---
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
{
    document.Components ??= new OpenApiComponents();
    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>(); // ← agregar esta línea

    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá tu JWT token (sin 'Bearer ' delante)"
    };
    return Task.CompletedTask;
});
});

// Modules Registration
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCalorieTrackingModule(builder.Configuration);
// builder.Services.AddAnalyticsModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
     app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "NutriMetrics API v1");
        options.RoutePrefix = "swagger";
    });
    
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();