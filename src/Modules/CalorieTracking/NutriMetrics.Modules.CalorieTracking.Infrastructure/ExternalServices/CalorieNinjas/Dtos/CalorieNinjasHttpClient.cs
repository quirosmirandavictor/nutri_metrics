namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.ExternalServices.CalorieNinjas;

using System.Net.Http.Json;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.ExternalServices.CalorieNinjas.Dtos;

public class CalorieNinjasHttpClient : INutritionApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITranslationService _translationService;

    public CalorieNinjasHttpClient(HttpClient httpClient, ITranslationService translationService)
    {
        _httpClient = httpClient;
        _translationService = translationService;
    }

    public async Task<IEnumerable<FoodItem>> SearchFoodAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<FoodItem>();
        }

        // 1. Translate the query to English using the translation service
        var englishQuery = await _translationService.TranslateToEnglishAsync(query, cancellationToken);

        // 2. Query CalorieNinjas with the text in English
        var response = await _httpClient.GetFromJsonAsync<CalorieNinjasResponseDto>(
            $"v1/nutrition?query={Uri.EscapeDataString(englishQuery)}", 
            cancellationToken
        );

        if (response?.Items == null || response.Items.Count == 0)
        {
            return Enumerable.Empty<FoodItem>();
        }

        // 3. Map response to Domain
        return response.Items.Select(item => new FoodItem(
            userId: Guid.Empty, // Temporary - these are search results, not persisted
            name: item.Name,
            calories: item.Calories,
            proteinGrams: item.ProteinG,
            fatGrams: item.FatTotalG,
            carbohydratesGrams: item.CarbohydratesTotalG,
            servingSizeGrams: item.ServingSizeG
        ));
    }
}