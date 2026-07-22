using NutriMetrics.Modules.CalorieTracking.Infrastructure;
using NutriMetrics.Modules.Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- DI Container ---
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Modules Registration
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCalorieTrackingModule(builder.Configuration);
// builder.Services.AddAnalyticsModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();