using Microsoft.EntityFrameworkCore;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.Database;

namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Repositories;

public class FoodItemRepository : IFoodItemRepository
{
    private readonly CalorieTrackingDbContext _dbContext;

    public FoodItemRepository(CalorieTrackingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(FoodItem foodItem, CancellationToken cancellationToken = default)
    {
        _dbContext.FoodItems.Add(foodItem);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<FoodItem>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FoodItems
            .Where(f => f.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<FoodItem?> GetByIdAndUserAsync(Guid itemId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FoodItems
            .FirstOrDefaultAsync(f => f.Id == itemId && f.UserId == userId, cancellationToken);
    }
}
