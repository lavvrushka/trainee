using FluentValidation;
using OfficesManagement.Core.Common.Validation; 
namespace OfficesManagement.API.Extensions;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LocationRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateOfficeRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateOfficeRequestValidator>();

        return services;
    }
}
