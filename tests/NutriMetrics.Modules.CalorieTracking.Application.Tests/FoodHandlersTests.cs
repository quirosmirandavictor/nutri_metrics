using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.AddFoodItem;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.DeleteFoodItem;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.GetFoodItemsByDateRange;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.SearchFood;
using NutriMetrics.Modules.CalorieTracking.Domain.Contracts;
using NutriMetrics.Modules.CalorieTracking.Domain.Entities;

namespace NutriMetrics.Modules.CalorieTracking.Application.Tests;

public class FoodHandlersTests
{
    [Fact]
    public async Task AddFoodItem_WithValidUserClaim_PersistsEntity()
    {
        var repository = new Mock<IFoodItemRepository>();
        var accessor = BuildAccessorWithUser(Guid.NewGuid());
        var handler = new AddFoodItemCommandHandler(repository.Object, accessor);

        FoodItem? created = null;
        repository
            .Setup(x => x.AddAsync(It.IsAny<FoodItem>(), It.IsAny<CancellationToken>()))
            .Callback<FoodItem, CancellationToken>((item, _) => created = item)
            .Returns(Task.CompletedTask);

        var command = new AddFoodItemCommand("Apple", 95, 0.5, 25, 0.3);

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        Assert.NotNull(created);
        Assert.Equal("Apple", created!.Name);
        repository.Verify(x => x.AddAsync(It.IsAny<FoodItem>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddFoodItem_WithoutValidUserClaim_ThrowsInvalidOperationException()
    {
        var repository = new Mock<IFoodItemRepository>();
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var handler = new AddFoodItemCommandHandler(repository.Object, accessor);

        var command = new AddFoodItemCommand("Apple", 95, 0.5, 25, 0.3);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteFoodItem_WhenOwned_DeletesAndReturnsId()
    {
        var userId = Guid.NewGuid();
        var item = new FoodItem(userId, "Rice", 130, 2.5, 0.2, 28, 100);
        var repository = new Mock<IFoodItemRepository>();
        repository
            .Setup(x => x.GetByIdAsync(item.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var accessor = BuildAccessorWithUser(userId);
        var handler = new DeleteFoodItemCommandHandler(repository.Object, accessor);

        var result = await handler.Handle(new DeleteFoodItemCommand(item.Id, userId), CancellationToken.None);

        Assert.Equal(item.Id, result);
        repository.Verify(x => x.DeleteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchFood_MapsDomainEntitiesToResponseDtos()
    {
        var nutritionClient = new Mock<INutritionApiClient>();
        nutritionClient
            .Setup(x => x.SearchFoodAsync("apple", It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new FoodItem(Guid.NewGuid(), "apple", 95, 0.5, 0.3, 25, 182),
                new FoodItem(Guid.NewGuid(), "banana", 105, 1.3, 0.4, 27, 118)
            ]);

        var handler = new SearchFoodQueryHandler(nutritionClient.Object);

        var result = (await handler.Handle(new SearchFoodQuery("apple"), CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("apple", result[0].Name);
        Assert.Equal(95, result[0].Calories);
        Assert.Equal("banana", result[1].Name);
    }

    [Fact]
    public async Task GetFoodItemsByDateRange_MapsAllFields()
    {
        var userId = Guid.NewGuid();
        var start = DateTime.UtcNow.Date.AddDays(-1);
        var end = DateTime.UtcNow.Date.AddDays(1);
        var createdAt = DateTime.UtcNow;

        var repository = new Mock<IFoodItemRepository>();
        repository
            .Setup(x => x.GetFoodItemsByDateRangeAsync(userId, start, end, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new FoodItem(userId, "Oats", 389, 16.9, 6.9, 66.3, 100, createdAt)]);

        var accessor = BuildAccessorWithUser(userId);
        var handler = new GetFoodItemsByDateRangeQueryHandler(repository.Object, accessor);

        var result = await handler.Handle(new GetFoodItemsByDateRangeQuery(userId, start, end), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Oats", result[0].Name);
        Assert.Equal(389, result[0].Calories);
        Assert.Equal(createdAt, result[0].CreatedAt);
    }

    private static IHttpContextAccessor BuildAccessorWithUser(Guid userId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        ],
        "TestAuth");

        return new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }
}
