namespace NutriMetrics.Modules.CalorieTracking.Infrastructure.ExternalServices.CalorieNinjas.Dtos;

using System.Text.Json.Serialization;

public class CalorieNinjasItemDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("calories")]
    public double Calories { get; set; }

    [JsonPropertyName("protein_g")]
    public double ProteinG { get; set; }

    [JsonPropertyName("fat_total_g")]
    public double FatTotalG { get; set; }

    [JsonPropertyName("carbohydrates_total_g")]
    public double CarbohydratesTotalG { get; set; }

    [JsonPropertyName("serving_size_g")]
    public double ServingSizeG { get; set; }
}