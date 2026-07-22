using MediatR;
using NutriMetrics.Modules.Identity.Application.Responses;

namespace NutriMetrics.Modules.Identity.Application.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string PasswordConfirm) : IRequest<AuthResponse>;
