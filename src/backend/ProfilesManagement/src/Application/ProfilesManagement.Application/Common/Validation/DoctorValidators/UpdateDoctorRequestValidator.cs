using FluentValidation;
using ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
namespace ProfilesManagement.Application.Common.Validation.DoctorValidators;

public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
{
    public UpdateDoctorRequestValidator()
    {
        RuleFor(r => r.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Doctor Id is required. Please provide a valid identifier.");

        RuleFor(r => r.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("First Name is required and cannot be empty.")
            .MaximumLength(100)
            .WithMessage("First Name must not exceed 100 characters.");

        RuleFor(r => r.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Last Name is required and cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Last Name must not exceed 100 characters.");

        RuleFor(r => r.MiddleName)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(100)
            .WithMessage("Middle Name must not exceed 100 characters.");

        RuleFor(r => r.AccountId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Account Id is required. Please provide a valid account identifier.");

        RuleFor(r => r.CareerStartYear)
            .Cascade(CascadeMode.Stop)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Career Start Year cannot be in the future. Please provide a valid date.");

        RuleFor(r => r.Status)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Status cannot be empty.")
            .SetValidator(new EmploymentStatusRequestValidator());

        RuleFor(r => r.OfficeId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Office Id has been provided but is empty. Please provide a valid Office Id.")
            .When(r => r.OfficeId.HasValue);

        RuleFor(r => r.SpecializationId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Specialization Id has been provided but is empty. Please provide a valid Specialization Id.")
            .When(r => r.SpecializationId.HasValue);
    }
}
