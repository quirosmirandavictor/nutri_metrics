namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.Services;

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;

public sealed class LibreTranslateTranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LibreTranslateTranslationService> _logger;
    private readonly string _sourceLanguage;
    private readonly string _targetLanguage;
    private readonly string _apiKey;

    public LibreTranslateTranslationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LibreTranslateTranslationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _sourceLanguage = configuration["Translation:LibreTranslate:SourceLanguage"] ?? "es";
        _targetLanguage = configuration["Translation:LibreTranslate:TargetLanguage"] ?? "en";
        _apiKey = configuration["Translation:LibreTranslate:ApiKey"] ?? string.Empty;
    }

    public async Task<string> TranslateToEnglishAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var translated = await TranslateRawAsync(text, cancellationToken);
        return string.IsNullOrWhiteSpace(translated) ? text : translated;
    }

    private async Task<string> TranslateRawAsync(string text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var request = new LibreTranslateRequest
        {
            Q = text,
            Source = _sourceLanguage,
            Target = _targetLanguage,
            Format = "text",
            ApiKey = string.IsNullOrWhiteSpace(_apiKey) ? null : _apiKey
        };

        // Retry once for transient networking failures.
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("/translate", request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning(
                        "LibreTranslate returned {StatusCode}. Attempt {Attempt}. Body: {Body}",
                        (int)response.StatusCode,
                        attempt,
                        body);

                    if (attempt == 2)
                    {
                        return text;
                    }

                    continue;
                }

                var payload = await response.Content.ReadFromJsonAsync<LibreTranslateResponse>(cancellationToken: cancellationToken);
                if (string.IsNullOrWhiteSpace(payload?.TranslatedText))
                {
                    _logger.LogWarning("LibreTranslate response did not include translatedText. Falling back to original query.");
                    return text;
                }

                return payload.TranslatedText;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "LibreTranslate HTTP request failed on attempt {Attempt}.", attempt);
                if (attempt == 2)
                {
                    return text;
                }
            }
            catch (NotSupportedException ex)
            {
                _logger.LogWarning(ex, "LibreTranslate response content type is not supported.");
                return text;
            }
            catch (System.Text.Json.JsonException ex)
            {
                _logger.LogWarning(ex, "LibreTranslate response JSON is invalid.");
                return text;
            }
        }

        return text;
    }
    private sealed class LibreTranslateRequest
    {
        [JsonPropertyName("q")]
        public string Q { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = "es";

        [JsonPropertyName("target")]
        public string Target { get; set; } = "en";

        [JsonPropertyName("format")]
        public string Format { get; set; } = "text";

        [JsonPropertyName("api_key")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ApiKey { get; set; }
    }

    private sealed class LibreTranslateResponse
    {
        [JsonPropertyName("translatedText")]
        public string? TranslatedText { get; set; }
    }
}