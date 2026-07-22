using Microsoft.AspNetCore.Identity;
using NutriMetrics.Modules.Identity.Domain.Contracts;
using NutriMetrics.Modules.Identity.Domain.Entities;

namespace NutriMetrics.Modules.Identity.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<User> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<(bool success, string message, string? token, Guid? userId)> RegisterAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return (false, "El usuario ya existe.", null, null);

            // Create new user
            var user = new User
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            // Create user with password
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Error al registrar: {errors}", null, null);
            }

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!);
            return (true, "Usuario registrado exitosamente.", token, user.Id);
        }
        catch (Exception ex)
        {
            return (false, $"Error en registro: {ex.Message}", null, null);
        }
    }

    public async Task<(bool success, string message, string? token, Guid? userId)> LoginAsync(
        string email, 
        string password, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "Credenciales inválidas.", null, null);

            // Check if user is active
            if (!user.IsActive)
                return (false, "El usuario está inactivo.", null, null);

            // Check password
            var passwordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!passwordValid)
                return (false, "Credenciales inválidas.", null, null);

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!);
            return (true, "Login exitoso.", token, user.Id);
        }
        catch (Exception ex)
        {
            return (false, $"Error en login: {ex.Message}", null, null);
        }
    }
}
