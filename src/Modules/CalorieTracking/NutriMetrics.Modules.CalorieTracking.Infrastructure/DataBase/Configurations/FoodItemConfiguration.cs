using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Database.Configurations;

public class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
{
    public void Configure(EntityTypeBuilder<FoodItem> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.Calories)
            .HasPrecision(10, 2);

        builder.Property(f => f.ProteinGrams)
            .HasPrecision(10, 2);

        builder.Property(f => f.FatGrams)
            .HasPrecision(10, 2);

        builder.Property(f => f.CarbohydratesGrams)
            .HasPrecision(10, 2);

        builder.Property(f => f.ServingSizeGrams)
            .HasPrecision(10, 2);

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime(6)")
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasIndex(f => f.UserId);

        builder.ToTable("FoodItems");
    }
}
