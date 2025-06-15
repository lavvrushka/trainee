using FluentValidation;
using ProfilesManagement.Application.Common.Validation.DoctorValidators;
using ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;
using ProfilesManagement.Application.Common.Validation.PatientValidators;
using ProfilesManagement.Application.Common.Validation.ReceptionistValidators;
using ProfilesManagement.Application.Common.Validation.SpecializationValidators;
namespace ProfilesManagement.API.Extensions;

public static class ValidationServiceCollectionExtensions
{
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateDoctorRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateDoctorRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreatePatientRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdatePatientRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateReceptionistRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateReceptionistRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateEmploymentStatusRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<FilterEmploymentStatusByNameRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateEmploymentStatusRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<CreateSpecializationRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateSpecializationRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<FilterSpecializationsByNameRequestValidator>();

        return services;
    }
}
