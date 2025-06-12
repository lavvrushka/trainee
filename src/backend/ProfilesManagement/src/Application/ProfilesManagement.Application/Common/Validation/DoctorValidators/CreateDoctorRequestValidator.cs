using FluentValidation;
using ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
namespace ProfilesManagement.Application.Common.Validation.DoctorValidators;

public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
{
    public CreateDoctorRequestValidator()
    {

        RuleFor(r => r.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("FirstName is required.")
            .Length(1, 100)
            .WithMessage("FirstName must be between 1 and 100 characters.");

        RuleFor(r => r.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("LastName is required.")
            .Length(1, 100)
            .WithMessage("LastName must be between 1 and 100 characters.");

        RuleFor(r => r.MiddleName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("MiddleName must be provided (empty string if none).")
            .MaximumLength(100)
            .WithMessage("MiddleName must not exceed 100 characters.");

        RuleFor(r => r.AccountId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("AccountId is required.");

        RuleFor(r => r.CareerStartYear)
            .Cascade(CascadeMode.Stop)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage(r => $"CareerStartYear ({r.CareerStartYear:yyyy-MM-dd}) cannot be in the future.")
            .GreaterThanOrEqualTo(new DateTime(1930, 1, 1))
            .WithMessage("CareerStartYear must be no earlier than January 1, 1930.");

        RuleFor(r => r.Status)
            .NotNull().WithMessage("Status cannot be empty.")
            .SetValidator(new EmploymentStatusRequestValidator());

        RuleFor(r => r.OfficeId)
            .Cascade(CascadeMode.Stop)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("OfficeId, if specified, must be a non-empty GUID.");

        RuleFor(r => r.SpecializationId)
            .Cascade(CascadeMode.Stop)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("SpecializationId, if specified, must be a non-empty GUID.");

        RuleFor(r => r.ImageId)
            .Cascade(CascadeMode.Stop)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("ImageId, if specified, must be a non-empty GUID.");
    }
}
