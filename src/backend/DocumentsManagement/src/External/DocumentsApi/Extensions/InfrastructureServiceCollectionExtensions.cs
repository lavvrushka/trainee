using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Repositories;
using Microsoft.Extensions.Options;

namespace DocumentsAPI.Extensions;

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

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();

        return services;
    }
}
