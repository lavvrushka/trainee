using FluentValidation;
using ProfilesManagement.Application.UseCases.SpecializationUseCases;
namespace ProfilesManagement.Application.Common.Validation.SpecializationValidators;

public class UpdateSpecializationRequestValidator: AbstractValidator<UpdateSpecializationRequest>
{
    public UpdateSpecializationRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be at most 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must be at most 1000 characters.")
            .When(x => x.Description != null);
    }
}
