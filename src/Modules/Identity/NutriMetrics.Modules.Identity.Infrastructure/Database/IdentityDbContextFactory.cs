using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NutriMetrics.Modules.Identity.Infrastructure.Database;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var apiConfigPath = ResolveApiConfigPath();
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var userSecretsPath = ResolveUserSecretsPath();

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(apiConfigPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        if (!string.IsNullOrWhiteSpace(userSecretsPath))
        {
            configurationBuilder.AddJsonFile(userSecretsPath, optional: true, reloadOnChange: false);
        }

        var configuration = configurationBuilder.Build();

        var connectionString = ResolveConnectionString(configuration);

        var serverVersion = ServerVersion.AutoDetect(connectionString);

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseMySql(connectionString, serverVersion);

        return new IdentityDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiConfigPath()
    {
        var current = Directory.GetCurrentDirectory();
        var candidate = Path.GetFullPath(Path.Combine(current, "src", "NutriMetrics.Api"));
        if (File.Exists(Path.Combine(candidate, "appsettings.json")))
        {
            return candidate;
        }

        return current;
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(connectionString)
            && !connectionString.Contains("YOUR_USER", StringComparison.OrdinalIgnoreCase)
            && !connectionString.Contains("YOUR_PASSWORD", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var mysqlPassword = configuration["MYSQL_ROOT_PASSWORD"];
        var mysqlDatabase = configuration["MYSQL_DATABASE"] ?? "nutrimetrics_calorietracking";
        if (!string.IsNullOrWhiteSpace(mysqlPassword))
        {
            return $"Server=localhost;Port=3306;Database={mysqlDatabase};User=root;Password={mysqlPassword};";
        }

        throw new InvalidOperationException(
            "Unable to resolve a valid connection string for design-time migrations. " +
            "Set ConnectionStrings__Default or configure user-secrets for the API project.");
    }

    private static string? ResolveUserSecretsPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrWhiteSpace(appData))
        {
            return null;
        }

        var path = Path.Combine(appData, "Microsoft", "UserSecrets", "e867b38a-a24a-4899-b904-be655a8f4198", "secrets.json");
        return File.Exists(path) ? path : null;
    }
}
