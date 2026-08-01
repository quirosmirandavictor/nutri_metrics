using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.AddFoodItem;
using NutriMetrics.Modules.Identity.Infrastructure.Database;
using NutriMetrics.Modules.CalorieTracking.Infrastructure.Database;

namespace NutriMetrics.Api.IntegrationTests;

public class FoodControllerIntegrationTests : IClassFixture<FoodApiFactory>
{
    private readonly HttpClient _client;

    public FoodControllerIntegrationTests(FoodApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddFood_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/food", new AddFoodItemRequest("Apple", 95, 0.5, 25, 0.3));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_ThenAddFood_ThenQueryByDateRange_ReturnsCreatedItem()
    {
        var email = $"int-{Guid.NewGuid():N}@test.local";
        var password = "Test1234!";

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email, password, passwordConfirm = password });

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registerJson = await registerResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = registerJson.GetProperty("token").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createdAt = DateTime.UtcNow;
        var addResponse = await _client.PostAsJsonAsync(
            "/api/food",
            new AddFoodItemRequest("Greek Yogurt", 150, 15, 10, 5, createdAt));

        Assert.Equal(HttpStatusCode.Created, addResponse.StatusCode);

        var startDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O");
        var endDate = new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("O");
        var queryResponse = await _client.GetAsync($"/api/food/by-date-range?startDate={Uri.EscapeDataString(startDate)}&endDate={Uri.EscapeDataString(endDate)}");

        Assert.Equal(HttpStatusCode.OK, queryResponse.StatusCode);

        var items = await queryResponse.Content.ReadFromJsonAsync<List<FoodItemDto>>();
        Assert.NotNull(items);
    }

    public record FoodItemDto(Guid Id, string Name, double Calories, double Protein, double Carbs, double Fat, DateTime CreatedAt);
}

public sealed class FoodApiFactory : WebApplicationFactory<Program>
{
    public FoodApiFactory()
    {
        Environment.SetEnvironmentVariable("Database__AutoInitialize", "false");
        Environment.SetEnvironmentVariable("Database__MySqlServerVersion", "8.0.36");
        Environment.SetEnvironmentVariable("Http__UseHttpsRedirection", "false");
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", "Server=localhost;Port=3306;Database=nutrimetrics_test;User=root;Password=root;");
        Environment.SetEnvironmentVariable("Jwt__Secret", "this-is-a-test-jwt-secret-with-more-than-32-characters");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "NutriMetrics.Tests");
        Environment.SetEnvironmentVariable("Jwt__Audience", "NutriMetrics.Tests.Client");
        Environment.SetEnvironmentVariable("Jwt__ExpirationMinutes", "60");
        Environment.SetEnvironmentVariable("CalorieNinjas__ApiKey", "test-key");
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:AutoInitialize"] = "false",
                ["Database:MySqlServerVersion"] = "8.0.36",
                ["Http:UseHttpsRedirection"] = "false",
                ["ConnectionStrings:Default"] = "Server=localhost;Port=3306;Database=nutrimetrics_test;User=root;Password=root;",
                ["Jwt:Secret"] = "this-is-a-test-jwt-secret-with-more-than-32-characters",
                ["Jwt:Issuer"] = "NutriMetrics.Tests",
                ["Jwt:Audience"] = "NutriMetrics.Tests.Client",
                ["Jwt:ExpirationMinutes"] = "60",
                ["CalorieNinjas:ApiKey"] = "test-key"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<IdentityDbContext>));
            services.RemoveAll(typeof(DbContextOptions<CalorieTrackingDbContext>));

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseInMemoryDatabase($"identity-{Guid.NewGuid():N}"));

            services.AddDbContext<CalorieTrackingDbContext>(options =>
                options.UseInMemoryDatabase($"calorie-{Guid.NewGuid():N}"));

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            scope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.EnsureCreated();
            scope.ServiceProvider.GetRequiredService<CalorieTrackingDbContext>().Database.EnsureCreated();
        });
    }
}
