using MyApp.Mediator.Behaviors;
using MyMediator.Behaviors;
using MyMediator.Interfaces;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
using ProfilesManagement.Application.UseCases.PatientUseCases;
using ProfilesManagement.Application.UseCases.ReceptionistUseCases;
namespace ProfilesManagement.API.Extensions;

public static class ApplicationCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddLogging();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<CreatePatientHandler>();
        services.AddScoped<UpdatePatientHandler>();
        services.AddScoped<GetAllPatientsHandler>();
        services.AddScoped<GetPatientByIdHandler>();
        services.AddScoped<DeletePatientHandler>();

        services.AddScoped<CreateDoctorHandler>();
        services.AddScoped<UpdateDoctorHandler>();
        services.AddScoped<GetAllDoctorsHandler>();
        services.AddScoped<GetDoctorByIdHandler>();
        //services.AddScoped<FilterDoctorsByNameHandler>();
        services.AddScoped<DeleteDoctorHandler>();

        services.AddScoped<CreateReceptionistHandler>();
        services.AddScoped<UpdateReceptionistHandler>();
        services.AddScoped<GetAllReceptionistsHandler>();
        services.AddScoped<GetReceptionistByIdHandler>();
        //services.AddScoped<FilterReceptionistsByNameHandler>();
        services.AddScoped<DeleteReceptionistHandler>();

        services.AddScoped<CreateEmploymentStatusHandler>();
        services.AddScoped<UpdateEmploymentStatusHandler>();
        services.AddScoped<GetAllEmploymentStatusesHandler>();
        services.AddScoped<GetEmploymentStatusByIdHandler>();
        services.AddScoped<FilterEmploymentStatusByNameHandler>();
        services.AddScoped<DeleteEmploymentStatusHandler>();

        return services;
    }
}