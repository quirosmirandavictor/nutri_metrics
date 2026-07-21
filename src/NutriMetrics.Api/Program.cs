using NutriMetrics.Modules.CalorieTracking.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- DI Container ---
builder.Services.AddControllers();
builder.Services.AddOpenApi(); //

// Modules Registration
builder.Services.AddCalorieTrackingModule(builder.Configuration);
// builder.Services.AddAnalyticsModule(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();