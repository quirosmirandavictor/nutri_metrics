using Microsoft.AspNetCore.Identity;

namespace NutriMetrics.Modules.Identity.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
