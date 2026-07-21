namespace NutriMetrics.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using NutriMetrics.Modules.CalorieTracking.Application.FoodItems.Queries.SearchFood;

[ApiController]
[Route("api/[controller]")]
public class FoodController : ControllerBase
{
    private readonly ISender _mediator;

    public FoodController(ISender mediator)
    {
        _mediator = mediator;
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