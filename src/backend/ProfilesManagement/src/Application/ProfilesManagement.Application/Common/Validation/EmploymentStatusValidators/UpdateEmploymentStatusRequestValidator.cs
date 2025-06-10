using FluentValidation;
using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
namespace ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;

public class UpdateEmploymentStatusRequestValidator : AbstractValidator<UpdateEmploymentStatusRequest>
{
    public UpdateEmploymentStatusRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
                .WithMessage("EmploymentStatus Id is required.");

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