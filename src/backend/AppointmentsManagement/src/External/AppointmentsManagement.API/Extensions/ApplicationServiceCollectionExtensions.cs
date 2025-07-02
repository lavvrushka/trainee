using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using AppointmentsManagement.Application.UseCases.ResultUseCases;
using MyApp.Mediator.Behaviors;
using MyMediator.Behaviors;
using MyMediator.Extensions;
using MyMediator.Interfaces;
using System.Reflection;

namespace AppointmentsManagement.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddLogging();

        services.AddMyMediator(Assembly.GetExecutingAssembly());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<CheckDoctorAvailabilityHandler>();
        services.AddScoped<CreateAppointmentHandler>();
        services.AddScoped<DeleteAppointmentHandler>();
        services.AddScoped<GetAllAppointmentsHandler>();
        services.AddScoped<GetAppointmentByIdHandler>();
        services.AddScoped<GetAppointmentsByDoctorAndDateHandler>();
        services.AddScoped<GetAppointmentsByPatientHandler>();
        services.AddScoped<GetAppointmentsByServiceHandler>();
        services.AddScoped<GetApprovedAppointmentsByPatientHandler>();
        services.AddScoped<GetPendingAppointmentsByDoctorHandler>();
        services.AddScoped<GetUpcomingAppointmentsHandler>();
        services.AddScoped<UpdateAppointmentHandler>();

        services.AddScoped<CreateResultHandler>();
        services.AddScoped<DeleteResultHandler>();
        services.AddScoped<GetAllResultsHandler>();
        services.AddScoped<GetRecentResultsHandler>();
        services.AddScoped<GetResultByAppointmentHandler>();
        services.AddScoped<GetResultByIdHandler>();
        services.AddScoped<UpdateResultHandler>();

        return services;
    }
}
