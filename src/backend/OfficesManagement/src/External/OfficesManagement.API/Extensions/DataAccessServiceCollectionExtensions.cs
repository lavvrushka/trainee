using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Infrastructure.Persistence.Contexts;
using OfficesManagement.Infrastructure.Persistence.Repositories;
namespace OfficesManagement.API.Extensions;

public static class DataAccessServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IOfficeRepository, OfficeRepository>();

        return services;
    }
}
