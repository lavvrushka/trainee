using FluentValidation;
using ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
namespace ProfilesManagement.Application.Common.Validation.DoctorValidators;

public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
{
    public UpdateDoctorRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Doctor Id is required.");

        RuleFor(r => r.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .MaximumLength(100).WithMessage("First Name must not exceed 100 characters.");

        RuleFor(r => r.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .MaximumLength(100).WithMessage("Last Name must not exceed 100 characters.");

        RuleFor(r => r.MiddleName)
            .MaximumLength(100).WithMessage("Middle Name must not exceed 100 characters.");

        RuleFor(r => r.AccountId)
            .NotEmpty().WithMessage("Account Id is required.");

        RuleFor(r => r.CareerStartYear)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Career Start Year cannot be in the future.");

        // вместо Status — StatusId
        RuleFor(r => r.StatusId)
            .NotEmpty().WithMessage("Status Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Status Id must be a non-empty GUID.");

        // OfficeId — если Guid (non-nullable)
        RuleFor(r => r.OfficeId)
            .NotEmpty().WithMessage("Office Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Office Id must be a non-empty GUID.");

        RuleFor(r => r.SpecializationId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("SpecializationId is required.")
                .Must(id => id != Guid.Empty).WithMessage("SpecializationId must be a non-empty GUID.");

        // ImageId — non-nullable Guid
        RuleFor(r => r.ImageId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ImageId is required.")
            .Must(id => id != Guid.Empty).WithMessage("ImageId must be a non-empty GUID.");
    }
}
