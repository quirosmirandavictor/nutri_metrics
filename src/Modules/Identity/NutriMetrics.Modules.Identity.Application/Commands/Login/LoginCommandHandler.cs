using MediatR;
using NutriMetrics.Modules.Identity.Application.Responses;
using NutriMetrics.Modules.Identity.Domain.Contracts;

namespace NutriMetrics.Modules.Identity.Application.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return new AuthResponse(false, "Email y contraseña son requeridos.", null, null);

        var (success, message, token, userId) = await _authService.LoginAsync(
            request.Email,
            request.Password,
            cancellationToken);

        return new AuthResponse(success, message, token, userId);
    }
}
