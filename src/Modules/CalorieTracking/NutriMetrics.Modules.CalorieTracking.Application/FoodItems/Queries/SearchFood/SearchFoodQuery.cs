namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.SearchFood;

using MediatR;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;

public record SearchFoodQuery(string Query) : IRequest<IEnumerable<FoodItemResponse>>;

public class SearchFoodQueryHandler : IRequestHandler<SearchFoodQuery, IEnumerable<FoodItemResponse>>
{
    private readonly INutritionApiClient _nutritionApiClient;

    public SearchFoodQueryHandler(INutritionApiClient nutritionApiClient)
    {
        _nutritionApiClient = nutritionApiClient;
    }

    public async Task<IEnumerable<FoodItemResponse>> Handle(SearchFoodQuery request, CancellationToken cancellationToken)
    {
        var foodItems = await _nutritionApiClient.SearchFoodAsync(request.Query, cancellationToken);

        // Mapping from Domain Entities to Application Response DTOs
        return foodItems.Select(item => new FoodItemResponse(
            item.Name,
            item.Calories,
            item.ProteinGrams,
            item.FatGrams,
            item.CarbohydratesGrams,
            item.ServingSizeGrams
        ));
    }
}