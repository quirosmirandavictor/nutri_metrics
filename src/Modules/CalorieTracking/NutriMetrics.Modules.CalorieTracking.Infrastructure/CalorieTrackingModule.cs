namespace NutriMetrics.Modules.CalorieTracking.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.ExternalServices.CalorieNinjas;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.Services;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.Repositories;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.Database;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.SearchFood;
public static class CalorieTrackingModule
{
    public static IServiceCollection AddCalorieTrackingModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database Configuration
        var connectionString = configuration.GetConnectionString("Default") 
            ?? throw new InvalidOperationException("ConnectionString 'Default' not found in configuration");
        var configuredServerVersion = configuration["Database:MySqlServerVersion"];
        var serverVersion = string.IsNullOrWhiteSpace(configuredServerVersion)
            ? ServerVersion.AutoDetect(connectionString)
            : new MySqlServerVersion(Version.Parse(configuredServerVersion));
        
        services.AddDbContext<CalorieTrackingDbContext>(options =>
            options.UseMySql(connectionString, serverVersion)
        );

        // 2. Injection of Repositories
        services.AddScoped<IFoodItemRepository, FoodItemRepository>();
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(SearchFoodQuery).Assembly)
        );

        // 3. Injection of Services
        services.AddHttpClient<ITranslationService, LibreTranslateTranslationService>(client =>
        {
            var baseUrl = configuration["Translation:LibreTranslate:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                client.BaseAddress = new Uri(baseUrl);
            }

            var timeoutMs = configuration.GetValue("Translation:LibreTranslate:TimeoutMs", 5000);
            client.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
        });
        services.AddHttpClient<INutritionApiClient, CalorieNinjasHttpClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.calorieninjas.com");
            
            // La API key se tomará del appsettings.json o Variables de Entorno
            var apiKey = configuration["CalorieNinjas:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
            }
        });
        return services;
    }
}
