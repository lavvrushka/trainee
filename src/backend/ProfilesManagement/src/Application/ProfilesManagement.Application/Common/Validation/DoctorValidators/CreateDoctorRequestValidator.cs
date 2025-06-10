using FluentValidation;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
namespace ProfilesManagement.Application.Common.Validation.DoctorValidators;

public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
{
    public CreateDoctorRequestValidator()
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(r => r.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(r => r.MiddleName)
            .MaximumLength(100);

        RuleFor(r => r.AccountId)
            .NotEmpty();

        RuleFor(r => r.CareerStartYear)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Career start year cannot be in the future.");

        RuleFor(r => r.Status)
            .IsInEnum();
    }
}
