using MediatR;
using NutriMetrics.Modules.Identity.Application.Responses;

namespace NutriMetrics.Modules.Identity.Application.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;
