namespace NutriMetrics.Modules.CalorieTracking.Domain.Contracts;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

public interface INutritionApiClient
{
    Task<IEnumerable<FoodItem>> SearchFoodAsync(string query, CancellationToken cancellationToken = default);
}