using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

namespace NutriMetrics.Modules.CalorieTracking.Domain.Contracts;

public interface IFoodItemRepository
{
    Task AddAsync(FoodItem foodItem, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FoodItem>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FoodItem?> GetByIdAndUserAsync(Guid itemId, Guid userId, CancellationToken cancellationToken = default);
}
