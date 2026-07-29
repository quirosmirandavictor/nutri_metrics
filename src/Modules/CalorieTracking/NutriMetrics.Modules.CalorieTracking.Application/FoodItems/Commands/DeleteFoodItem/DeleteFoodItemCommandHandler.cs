using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;


namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.DeleteFoodItem;

public class DeleteFoodItemCommandHandler : IRequestHandler<DeleteFoodItemCommand, Guid>
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteFoodItemCommandHandler(IFoodItemRepository foodItemRepository, IHttpContextAccessor httpContextAccessor)
    {
        _foodItemRepository = foodItemRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid> Handle(DeleteFoodItemCommand request,  CancellationToken cancellationToken)
    {

        // Retrieve the food item to ensure it belongs to the user
        FoodItem? foodItem = await _foodItemRepository.GetByIdAsync(request.Id, request.UserId, cancellationToken);
        if (foodItem == null || foodItem.UserId != request.UserId)
            throw new InvalidOperationException("Food item not found or does not belong to the user");

        await _foodItemRepository.DeleteAsync(foodItem, cancellationToken);
        await _foodItemRepository.SaveChangesAsync(cancellationToken);

        return foodItem.Id;
    }
}