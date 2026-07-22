namespace NutriMetrics.Modules.Identity.Application.Responses;

public record AuthResponse(
    bool Success,
    string Message,
    string? Token,
    Guid? UserId
);
