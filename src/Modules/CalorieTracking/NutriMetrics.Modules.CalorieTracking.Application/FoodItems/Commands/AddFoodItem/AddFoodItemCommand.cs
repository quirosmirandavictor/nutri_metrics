using MediatR;

namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.AddFoodItem;

// Request DTO recibido desde la API
public record AddFoodItemRequest(string Name, double Calories, double Protein, double Carbs, double Fat, DateTime? CreatedAt = null);

// Command para MediatR que retorna el ID del elemento creado
public record AddFoodItemCommand(
    string Name, 
    double Calories, 
    double Protein, 
    double Carbs, 
    double Fat,
    DateTime? CreatedAt = null) : IRequest<Guid>;