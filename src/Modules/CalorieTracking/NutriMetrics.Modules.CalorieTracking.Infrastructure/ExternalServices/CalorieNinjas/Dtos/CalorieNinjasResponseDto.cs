namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.ExternalServices.CalorieNinjas.Dtos;

using System.Text.Json.Serialization;

public class CalorieNinjasResponseDto
{
    [JsonPropertyName("items")]
    public List<CalorieNinjasItemDto> Items { get; set; } = [];
}