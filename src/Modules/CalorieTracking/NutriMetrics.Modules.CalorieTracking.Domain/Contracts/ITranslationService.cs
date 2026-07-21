namespace NutriMetrics.Modules.CalorieTracking.Domain.Contracts;

public interface ITranslationService
{
    Task<string> TranslateToEnglishAsync(string text, CancellationToken cancellationToken = default);
}