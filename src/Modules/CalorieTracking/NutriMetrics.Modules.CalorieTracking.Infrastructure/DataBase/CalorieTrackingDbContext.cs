using Microsoft.EntityFrameworkCore;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Database;

public class CalorieTrackingDbContext : DbContext
{
    public DbSet<FoodItem> FoodItems { get; set; }

    public CalorieTrackingDbContext(DbContextOptions<CalorieTrackingDbContext> options) 
        : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CalorieTrackingDbContext).Assembly);
    }
}