using FluentValidation;
using ProfilesManagement.Application.UseCases.ReceptionistUseCases;
namespace ProfilesManagement.Application.Common.Validation.ReceptionistValidators;

public class CreateReceptionistRequestValidator : AbstractValidator<CreateReceptionistRequest>
{
    public CreateReceptionistRequestValidator()
    {
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

        RuleFor(x => x.OfficeId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("If provided, Office Id must be a valid non-empty GUID.");
    }
}
