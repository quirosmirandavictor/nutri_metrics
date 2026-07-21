namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.SearchFood;

public record FoodItemResponse(
    string Name,
    double Calories,
    double ProteinGrams,
    double FatGrams,
    double CarbohydratesGrams,
    double ServingSizeGrams
);