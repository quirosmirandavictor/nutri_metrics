namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Services;

using GoogleTranslateFreeApi;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;

public class GoogleTranslationService : ITranslationService
{
    private readonly GoogleTranslator _translator;

    public GoogleTranslationService()
    {
        _translator = new GoogleTranslator();
    }

    public async Task<string> TranslateToEnglishAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        try
        {
            // translate the text to English using Google Translate API
            var result = await _translator.TranslateLiteAsync(text, Language.Auto, Language.English);
            return result.MergedTranslation;
        }
        catch
        {
            // if the translation fails due to network issues or quota limits, return the original text as a fallback
            return text;
        }
    }
}