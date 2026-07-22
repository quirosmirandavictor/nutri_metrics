namespace NutriMetrics.Modules.CalorieTracking.Domain.Entities;

public class FoodItem
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public double Calories { get; private set; }
    public double ProteinGrams { get; private set; }
    public double FatGrams { get; private set; }
    public double CarbohydratesGrams { get; private set; }
    public double ServingSizeGrams { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public FoodItem(
        Guid userId,
        string name, 
        double calories, 
        double proteinGrams, 
        double fatGrams, 
        double carbohydratesGrams, 
        double servingSizeGrams,
        DateTime createdAt = default)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Calories = calories;
        ProteinGrams = proteinGrams;
        FatGrams = fatGrams;
        CarbohydratesGrams = carbohydratesGrams;
        ServingSizeGrams = servingSizeGrams;
        CreatedAt = createdAt == default ? DateTime.UtcNow : createdAt;
    }
}