using MediatR;
using NutriMetrics.Modules.Identity.Application.Responses;
using NutriMetrics.Modules.Identity.Domain.Contracts;

namespace NutriMetrics.Modules.Identity.Application.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return new AuthResponse(false, "El email es requerido.", null, null);

        if (request.Password != request.PasswordConfirm)
            return new AuthResponse(false, "Las contraseñas no coinciden.", null, null);

        if (request.Password?.Length < 8)
            return new AuthResponse(false, "La contraseña debe tener al menos 8 caracteres.", null, null);

        var (success, message, token, userId) = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return new AuthResponse(success, message, token, userId);
    }
}
