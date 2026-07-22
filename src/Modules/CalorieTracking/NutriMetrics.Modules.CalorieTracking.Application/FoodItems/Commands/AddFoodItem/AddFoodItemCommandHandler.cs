using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.AddFoodItem;

public class AddFoodItemCommandHandler : IRequestHandler<AddFoodItemCommand, Guid>
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddFoodItemCommandHandler(IFoodItemRepository foodItemRepository, IHttpContextAccessor httpContextAccessor)
    {
        _foodItemRepository = foodItemRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(AddFoodItemCommand request, CancellationToken cancellationToken)
    {
        // Get UserId from JWT token
        var userIdClaim = _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new InvalidOperationException("User ID not found in token");

        // Create FoodItem using the domain model constructor
        var foodItem = new FoodItem(
            userId: userId,
            name: request.Name,
            calories: request.Calories,
            proteinGrams: request.Protein,
            fatGrams: request.Fat,
            carbohydratesGrams: request.Carbs,
            servingSizeGrams: 100, // Default serving size
            createdAt: request.CreatedAt ?? default
        );

        await _foodItemRepository.AddAsync(foodItem, cancellationToken);
        await _foodItemRepository.SaveChangesAsync(cancellationToken);

        return foodItem.Id;
    }
}
