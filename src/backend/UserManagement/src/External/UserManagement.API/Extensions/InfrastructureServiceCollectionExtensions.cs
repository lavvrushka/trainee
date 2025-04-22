using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserManagement.Application.Common.Interfaces.IServices;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Infrastructure.Persistence.Configurations;
using UserManagement.Infrastructure.Persistence.Context;
using UserManagement.Infrastructure.Persistence.Repositories;
using UserManagement.Infrastructure.Services;
namespace UserManagement.API.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection("ConnectionStrings"));

        services.AddDbContext<UserManagementDbContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(dbOptions.DefaultConnection);
        });

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
        services.AddScoped<IEmailDeliveryService, EmailDeliveryService>();
        services.AddScoped<IPasswordResetService, PasswordResetService>();

        return services;
    }
}
