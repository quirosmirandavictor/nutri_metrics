using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.Database;
using NutriMetrics.Modules.Identity.Domain.Entities;
using NutriMetrics.Modules.Identity.Infrastructure.Database;

namespace NutriMetrics.Api.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var identityDb = services.GetRequiredService<IdentityDbContext>();
        var calorieDb = services.GetRequiredService<CalorieTrackingDbContext>();

        await identityDb.Database.MigrateAsync();
        await calorieDb.Database.MigrateAsync();

        await SeedTestUserAsync(services, app.Configuration, app.Logger);
    }

    private static async Task SeedTestUserAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var enableTestUser = configuration.GetValue<bool>("Seed:EnableTestUser");
        if (!enableTestUser)
        {
            return;
        }

        var email = configuration["Seed:TestUser:Email"];
        var password = configuration["Seed:TestUser:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Seed:EnableTestUser is enabled but seed credentials are missing. Set Seed:TestUser:Email and Seed:TestUser:Password.");
            return;
        }

        var userManager = services.GetRequiredService<UserManager<User>>();
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            logger.LogInformation("Test seed user already exists: {Email}", email);
            return;
        }

        var user = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Failed to seed test user {Email}: {Errors}", email, errors);
            return;
        }

        logger.LogInformation("Seeded test user: {Email}", email);
    }
}
