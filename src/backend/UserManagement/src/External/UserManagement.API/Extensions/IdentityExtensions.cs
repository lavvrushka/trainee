using Microsoft.AspNetCore.Identity;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Infrastructure.Persistence.Context;
namespace UserManagement.API.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services)
    {
        services.AddIdentity<Account, Role>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<UserManagementDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
