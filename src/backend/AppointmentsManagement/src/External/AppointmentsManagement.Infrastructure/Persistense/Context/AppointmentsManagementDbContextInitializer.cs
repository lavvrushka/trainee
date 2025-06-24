using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace AppointmentsManagement.Infrastructure.Persistense.Context;

public static class AppointmentsManagementDbContextInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppointmentsManagementDbContext>();

        await context.Database.MigrateAsync();
    }
}
