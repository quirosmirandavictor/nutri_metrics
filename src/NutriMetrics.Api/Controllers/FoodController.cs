namespace NutriMetrics.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.SearchFood;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Commands.AddFoodItem;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FoodController : ControllerBase
{
    private readonly ISender _mediator;

    public FoodController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddFoodItem([FromBody] AddFoodItemRequest request, CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("El nombre del alimento no puede estar vacío.");
        }

        var command = new AddFoodItemCommand(
            request.Name,
            request.Calories,
            request.Protein,
            request.Carbs,
            request.Fat,
            request.CreatedAt
        );

        var foodItemId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(AddFoodItem), new { id = foodItemId }, new { id = foodItemId });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("El término de búsqueda no puede estar vacío.");
        }

        var result = await _mediator.Send(new SearchFoodQuery(query), cancellationToken);
        return Ok(result);
    }
}