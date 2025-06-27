using DocumentsBusinessLogic.UseCases.DocumentsUseCases;
using DocumentsBusinessLogic.UseCases.ImagesUseCases;
using MyApp.Mediator.Behaviors;
using MyMediator.Behaviors;
using MyMediator.Interfaces;

namespace DocumentsAPI.Extensions;

public static class ApplicationCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddLogging();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<CreateDocumentHandler>();
        services.AddScoped<UpdateDocumentHandler>();
        services.AddScoped<GetAllDocumentsHandler>();
        services.AddScoped<GetDocumentByIdHandler>();
        services.AddScoped<SoftDeleteDocumentHandler>();
        services.AddScoped<DownloadDocumentHandler>();

        services.AddScoped<CreateImageHandler>();
        services.AddScoped<UpdateImageHandler>();
        services.AddScoped<GetAllImagesHandler>();
        services.AddScoped<GetImageByIdHandler>();

        return services;
    }
}
