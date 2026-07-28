using MediatR;

namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.GetFoodItemsByDateRange;

// Query for MediatR that returns a list of food items based on a search term
public record GetFoodItemsByDateRangeQuery(
    Guid UserId, 
    DateTime StartDate, 
    DateTime EndDate
) : IRequest<IReadOnlyList<FoodItemDto>>;
public record FoodItemDto(
    Guid Id,
    string Name,
    double Calories,
    double Protein,
    double Carbs,
    double Fat,
    DateTime CreatedAt
);