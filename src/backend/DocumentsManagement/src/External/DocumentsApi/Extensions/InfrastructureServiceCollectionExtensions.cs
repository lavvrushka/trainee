using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Options;
using DocumentsDataAccess.Persistence.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace DocumentsAPI.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresSettings>(configuration.GetSection("ConnectionStrings"));

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var dbOptions = serviceProvider.GetRequiredService<IOptions<PostgresSettings>>().Value;
            options.UseNpgsql(dbOptions.DefaultConnection);
        });

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();

        return services;
    }
}
