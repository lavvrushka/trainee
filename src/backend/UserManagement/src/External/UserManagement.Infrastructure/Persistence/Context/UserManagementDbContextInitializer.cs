using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Domain.Interfaces.Enums;
using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Infrastructure.Persistence.Context;

public static class UserManagementDbContextInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await context.Database.MigrateAsync();

        await SeedRolesAsync(context);
        await SeedAdminAsync(context, config);
    }

    public static async Task SeedRolesAsync(UserManagementDbContext context)
    {
        if (!await context.Roles.AnyAsync())
        {
            foreach (RoleType role in Enum.GetValues(typeof(RoleType)))
            {
                var roleName = role.ToString();
                var description = role switch
                {
                    RoleType.Admin => "System administrator with full access to all functions.",
                    RoleType.Patient => "Registered patient who can view and manage their medical records.",
                    RoleType.Doctor => "Medical professional with access to patient data and medical tools.",
                    RoleType.Receptionist => "Staff responsible for scheduling and administrative tasks.",
                    _ => throw new ArgumentOutOfRangeException(nameof(role), $"No description available for role: {role}")
                };

                var newRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = description
                };

                context.Roles.Add(newRole);
            }

            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedAdminAsync(UserManagementDbContext context, IConfiguration config)
    {
        var email = config["DefaultAdmin:Email"];
        var password = config["DefaultAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Admin credentials must be provided in configuration.");

        var userManager = context.GetService<UserManager<Account>>();
        var roleManager = context.GetService<RoleManager<Role>>();

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
            return;

        var newAdmin = new Account
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            EmailConfirmed = true,
            EmailVerifiedAt = DateTime.UtcNow,
            CreatedBy = "System",
            UpdatedBy = "System",
            BirthDate = new DateTime(2004, 12, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        var createResult = await userManager.CreateAsync(newAdmin, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create admin user: {errors}");
        }

        const string adminRoleName = "ADMIN";

        if (!await roleManager.RoleExistsAsync(adminRoleName))
        {
            await roleManager.CreateAsync(new Role
            {
                Name = adminRoleName,
                NormalizedName = adminRoleName.ToUpper(),
                Description = "System administrator with full access to all functions."
            });
        }

        var roleResult = await userManager.AddToRoleAsync(newAdmin, adminRoleName);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to assign role to admin user: {errors}");
        }
    }
}
