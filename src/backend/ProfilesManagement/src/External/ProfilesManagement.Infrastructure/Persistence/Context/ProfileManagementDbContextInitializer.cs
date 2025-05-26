using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace ProfilesManagement.Infrastructure.Persistence.Context;

public static class ProfileManagementDbContextInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProfileManagementDbContext>();
        await context.Database.MigrateAsync();
    }
}