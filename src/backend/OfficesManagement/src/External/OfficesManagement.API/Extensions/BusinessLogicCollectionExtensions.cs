using OfficesManagement.BuisnessLogic.Common.Interfaces.IServices;
using OfficesManagement.BuisnessLogic.Services;
namespace OfficesManagement.API.Extensions;

public static class BusinessLogicCollectionExtensions
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IOfficeService, OfficeService>();

        return services;
    }
}
