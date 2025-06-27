using DocumentsBusinessLogic.Common.Validation.DocumentsValidators;
using DocumentsBusinessLogic.Common.Validation.ImagesValidators;

namespace DocumentsAPI.Extensions;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateImageRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateImageRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateDocumentRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDocumentRequestValidator>();

        return services;
    }
}
