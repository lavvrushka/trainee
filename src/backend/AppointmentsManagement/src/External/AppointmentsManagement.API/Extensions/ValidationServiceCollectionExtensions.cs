using AppointmentsManagement.Application.Common.Validation.AppointmentValidators;
using AppointmentsManagement.Application.Common.Validation.ResultValidators;
using FluentValidation;

namespace AppointmentsManagement.API.Extensions;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateAppointmentRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateAppointmentRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateResultRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateResultRequestValidator>();

        return services;
    }
}
