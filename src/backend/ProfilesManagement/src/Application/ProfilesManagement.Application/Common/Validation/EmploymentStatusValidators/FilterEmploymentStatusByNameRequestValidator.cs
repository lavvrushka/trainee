using FluentValidation;
using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
namespace ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;

public class FilterEmploymentStatusByNameQueryValidator : AbstractValidator<FilterEmploymentStatusByNameRequest>
{
    public FilterEmploymentStatusByNameQueryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Search criteria is required for filtering.");
    }
}
