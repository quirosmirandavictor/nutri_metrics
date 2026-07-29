using MediatR;

namespace NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.DeleteFoodItem;

// Request DTO received from the API
public record DeleteFoodItemRequest(Guid Id ,Guid UserId);

// Command for MediatR that returns the ID of the deleted item
public record DeleteFoodItemCommand(Guid Id, Guid UserId) : IRequest<Guid>;
