namespace NutriMetrics.Modules.Identity.Domain.Contracts;

public interface IAuthService
{
    Task<(bool success, string message, string? token, Guid? userId)> RegisterAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken = default);

    Task<(bool success, string message, string? token, Guid? userId)> LoginAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken = default);
}

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email);
}
