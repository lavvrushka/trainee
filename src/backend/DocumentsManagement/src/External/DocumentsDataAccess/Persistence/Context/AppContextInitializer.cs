using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace DocumentsDataAccess.Persistence.Context;

public static class AppContextInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

    }
}
