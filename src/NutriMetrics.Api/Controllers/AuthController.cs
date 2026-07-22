using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMetrics.Modules.Identity.Application.Commands.Login;
using NutriMetrics.Modules.Identity.Application.Commands.Register;

namespace NutriMetrics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { success = false, message = "El email es requerido." });

        var command = new RegisterCommand(request.Email, request.Password, request.PasswordConfirm);
        var response = await _mediator.Send(command, cancellationToken);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { success = false, message = "Email y contraseña son requeridos." });

        var command = new LoginCommand(request.Email, request.Password);
        var response = await _mediator.Send(command, cancellationToken);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpGet("verify")]
    [Authorize]
    public IActionResult Verify()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Ok(new { message = "Token válido", userId });
    }
}

public record RegisterRequest(string Email, string Password, string PasswordConfirm);
public record LoginRequest(string Email, string Password);
