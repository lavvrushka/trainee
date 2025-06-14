using FluentValidation;
using ProfilesManagement.Application.UseCases.SpecializationUseCases;
namespace ProfilesManagement.Application.Common.Validation.SpecializationValidators;

public class FilterSpecializationsByNameRequestValidator: AbstractValidator<FilterSpecializationsByNameRequest>
{
    public FilterSpecializationsByNameRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Search term must not be empty.")
            .MaximumLength(200).WithMessage("Search term must be at most 200 characters.");
    }
}
