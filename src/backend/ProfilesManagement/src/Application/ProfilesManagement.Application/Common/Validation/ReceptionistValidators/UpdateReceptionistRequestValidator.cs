using FluentValidation;
using ProfilesManagement.Application.UseCases.ReceptionistUseCases;
namespace ProfilesManagement.Application.Common.Validation.ReceptionistValidators;

public class UpdateReceptionistRequestValidator : AbstractValidator<UpdateReceptionistRequest>
{
    public UpdateReceptionistRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Receptionist Id is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100).WithMessage("Middle name must not exceed 100 characters.");

        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account Id is required.");

        RuleFor(x => x.StatusId)
            .NotEmpty().WithMessage("Employment status Id is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Employment status Id must be a non-empty GUID.");


        RuleFor(r => r.OfficeId)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("SpecializationId is required.")
               .Must(id => id != Guid.Empty).WithMessage("SpecializationId must be a non-empty GUID.");

        RuleFor(r => r.ImageId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ImageId is required.")
            .Must(id => id != Guid.Empty).WithMessage("ImageId must be a non-empty GUID.");
    }

}
