using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using NutriMetrics.Modules.Identity.Domain.Entities;

#nullable disable

namespace NutriMetrics.Modules.Identity.Infrastructure.Database.Migrations
{
    [Migration("20260731235500_SeedLocalTestUser")]
    public class SeedLocalTestUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var userId = Guid.Parse("5E764297-5445-4D2E-8E5A-57C2A5E324D8");
            const string email = "test.user@nutrimetrics.local";
            const string normalizedEmail = "TEST.USER@NUTRIMETRICS.LOCAL";
            const string password = "Test1234!";

            var now = DateTime.UtcNow;
            var user = new User
            {
                Id = userId,
                UserName = email,
                NormalizedUserName = normalizedEmail,
                Email = email,
                NormalizedEmail = normalizedEmail,
                EmailConfirmed = true,
                CreatedAt = now,
                IsActive = true,
                LockoutEnabled = false,
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var passwordHasher = new PasswordHasher<User>();
            var passwordHash = passwordHasher.HashPassword(user, password);

            var createdAt = now.ToString("yyyy-MM-dd HH:mm:ss.ffffff");
            var escapedPasswordHash = passwordHash.Replace("'", "''");
            var escapedConcurrencyStamp = user.ConcurrencyStamp!.Replace("'", "''");
            var escapedSecurityStamp = user.SecurityStamp!.Replace("'", "''");

            migrationBuilder.Sql($@"
INSERT INTO AspNetUsers
(Id, AccessFailedCount, ConcurrencyStamp, CreatedAt, Email, EmailConfirmed, IsActive, LastLogin, LockoutEnabled, LockoutEnd, NormalizedEmail, NormalizedUserName, PasswordHash, PhoneNumber, PhoneNumberConfirmed, SecurityStamp, TwoFactorEnabled, UserName)
SELECT
'{userId}', 0, '{escapedConcurrencyStamp}', '{createdAt}', '{email}', 1, 1, NULL, 0, NULL, '{normalizedEmail}', '{normalizedEmail}', '{escapedPasswordHash}', NULL, 0, '{escapedSecurityStamp}', 0, '{email}'
WHERE NOT EXISTS (
    SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = '{normalizedEmail}'
);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE NormalizedEmail = 'TEST.USER@NUTRIMETRICS.LOCAL';");
        }
    }
}
