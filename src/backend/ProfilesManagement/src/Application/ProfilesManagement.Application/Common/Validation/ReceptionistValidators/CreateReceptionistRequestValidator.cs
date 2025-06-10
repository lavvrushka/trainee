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

        RuleFor(x => x.Status)
            .NotNull().WithMessage("Employment status is required.")
            .DependentRules(() =>
            {
                RuleFor(x => x.Status.Status)
                    .NotEmpty().WithMessage("Status field within EmploymentStatus is required.")
                    .MaximumLength(100).WithMessage("Status field must not exceed 100 characters.");
            });

        RuleFor(x => x.OfficeId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("If provided, Office Id must be a valid identifier.");
    }
}
