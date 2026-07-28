using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.GetFoodItemsByDateRange;

public class GetFoodItemsByDateRangeQueryHandler 
    : IRequestHandler<GetFoodItemsByDateRangeQuery, IReadOnlyList<FoodItemDto>>
{
        private readonly IFoodItemRepository _foodItemRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetFoodItemsByDateRangeQueryHandler(
        IFoodItemRepository foodItemRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _foodItemRepository = foodItemRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<FoodItemDto>> Handle(
        GetFoodItemsByDateRangeQuery request, 
        CancellationToken cancellationToken)
    { 

        IReadOnlyList<FoodItem> foodItems = await _foodItemRepository.GetFoodItemsByDateRangeAsync(
                    request.UserId,
                    request.StartDate,
                    request.EndDate,
                    cancellationToken);

       return foodItems.Select(f => new FoodItemDto(
           f.Id,
           f.Name,
           f.Calories,
           f.ProteinGrams,
           f.CarbohydratesGrams,
           f.FatGrams,
           f.CreatedAt
       )).ToList();
    }
}