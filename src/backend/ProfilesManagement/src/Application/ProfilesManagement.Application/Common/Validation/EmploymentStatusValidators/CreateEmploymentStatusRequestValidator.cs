using FluentValidation;
using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
namespace ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;

public class CreateEmploymentStatusRequestValidator : AbstractValidator<CreateEmploymentStatusRequest>
{
    public CreateEmploymentStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
                .WithMessage("Status is required.")
            .MaximumLength(100)
                .WithMessage("Status must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
                .WithMessage("Description is required.")
            .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.DateTime)
            .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("DateTime cannot be in the future.");
    }
}
