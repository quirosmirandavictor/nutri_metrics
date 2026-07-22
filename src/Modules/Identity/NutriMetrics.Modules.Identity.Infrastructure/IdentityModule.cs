using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NutriMetrics.Modules.Identity.Application.Commands.Login;
using NutriMetrics.Modules.Identity.Infrastructure.Database;
using NutriMetrics.Modules.Identity.Domain.Contracts;
using NutriMetrics.Modules.Identity.Domain.Entities;
using NutriMetrics.Modules.Identity.Infrastructure.Services;

namespace NutriMetrics.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database Configuration
        var connectionString = configuration.GetConnectionString("Default") 
            ?? throw new InvalidOperationException("ConnectionString 'Default' not found in configuration");
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseMySql(connectionString, serverVersion)
        );

        // 2. Identity Configuration
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Password requirements
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        });

        // 3. JWT Configuration
        var jwtSecret = configuration["Jwt:Secret"] ?? "your-very-long-secret-key-minimum-32-characters-required";
        var jwtIssuer = configuration["Jwt:Issuer"] ?? "NutriMetrics.Api";
        var jwtAudience = configuration["Jwt:Audience"] ?? "NutriMetrics.Client";
        var jwtExpirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");

        services.AddSingleton<IJwtTokenService>(new JwtTokenService(jwtSecret, jwtIssuer, jwtAudience, jwtExpirationMinutes));
        services.AddScoped<IAuthService, AuthService>();

        // 4. JWT Bearer Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var key = System.Text.Encoding.UTF8.GetBytes(jwtSecret);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        // 5. MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly)
        );

        // 6. HTTP Context Accessor
        services.AddHttpContextAccessor();

        return services;
    }
}
