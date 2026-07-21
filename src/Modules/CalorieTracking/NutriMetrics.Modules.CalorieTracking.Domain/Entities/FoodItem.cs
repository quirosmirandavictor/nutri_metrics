namespace NutriMetrics.Modules.CalorieTracking.Domain.Entities;

public class FoodItem
{
    public string Name { get; private set; }
    public double Calories { get; private set; }
    public double ProteinGrams { get; private set; }
    public double FatGrams { get; private set; }
    public double CarbohydratesGrams { get; private set; }
    public double ServingSizeGrams { get; private set; }

    public FoodItem(
        string name, 
        double calories, 
        double proteinGrams, 
        double fatGrams, 
        double carbohydratesGrams, 
        double servingSizeGrams)
    {
        Name = name;
        Calories = calories;
        ProteinGrams = proteinGrams;
        FatGrams = fatGrams;
        CarbohydratesGrams = carbohydratesGrams;
        ServingSizeGrams = servingSizeGrams;
    }
}