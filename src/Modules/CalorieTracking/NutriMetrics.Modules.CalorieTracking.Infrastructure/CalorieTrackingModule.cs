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
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        
        services.AddDbContext<CalorieTrackingDbContext>(options =>
            options.UseMySql(connectionString, serverVersion)
        );

        // 2. Injection of Repositories
        services.AddScoped<IFoodItemRepository, FoodItemRepository>();
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(SearchFoodQuery).Assembly)
        );

        // 3. Injection of Services
        services.AddSingleton<ITranslationService, GoogleTranslationService>();
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
