using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Database;

public class CalorieTrackingDbContextFactory : IDesignTimeDbContextFactory<CalorieTrackingDbContext>
{
    public CalorieTrackingDbContext CreateDbContext(string[] args)
    {
        // Load configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CalorieTrackingDbContext>();

        var connectionString = configuration.GetConnectionString("Default") 
            ?? throw new InvalidOperationException("ConnectionString 'Default' not found in configuration");
        
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        optionsBuilder.UseMySql(connectionString, serverVersion);

        return new CalorieTrackingDbContext(optionsBuilder.Options);
    }
}