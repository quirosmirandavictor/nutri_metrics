using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.AddFoodItem;

namespace NutriMetrics.Api.E2eTests;

public class FoodControllerE2eTests
{
    private static readonly Uri BaseAddress = new(Environment.GetEnvironmentVariable("NUTRIMETRICS_E2E_BASE_URL") ?? "http://localhost:8080");

    [Fact]
    public async Task VerifyEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/auth/verify");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_Login_AddFood_And_QueryByDateRange_WorksAgainstDockerStack()
    {
        using var client = CreateClient();

        var email = $"e2e-{Guid.NewGuid():N}@test.local";
        var password = "Test1234!";

        var registerResponse = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { email, password, passwordConfirm = password });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registerJson = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = registerJson.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createdAt = DateTime.UtcNow;
        var addResponse = await client.PostAsJsonAsync(
            "/api/food",
            new AddFoodItemRequest("Salmon", 208, 20, 0, 13, createdAt));

        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

        var startDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O");
        var endDate = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O");
        var queryResponse = await client.GetAsync($"/api/food/by-date-range?startDate={Uri.EscapeDataString(startDate)}&endDate={Uri.EscapeDataString(endDate)}");

        Assert.Equal(HttpStatusCode.OK, queryResponse.StatusCode);

        var items = await queryResponse.Content.ReadFromJsonAsync<List<FoodItemDto>>();
        Assert.NotNull(items);
        Assert.Contains(items!, i => i.Name == "Salmon");
    }

    private static HttpClient CreateClient()
    {
        return new HttpClient
        {
            BaseAddress = BaseAddress,
            Timeout = TimeSpan.FromSeconds(20)
        };
    }

    public record FoodItemDto(Guid Id, string Name, double Calories, double Protein, double Carbs, double Fat, DateTime CreatedAt);
}
