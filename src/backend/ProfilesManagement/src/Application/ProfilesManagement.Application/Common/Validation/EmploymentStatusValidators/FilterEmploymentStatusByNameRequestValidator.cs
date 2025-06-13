using FluentValidation;
using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
namespace ProfilesManagement.Application.Common.Validation.EmploymentStatusValidators;

public class FilterEmploymentStatusByNameRequestValidator : AbstractValidator<FilterEmploymentStatusByNameRequest>
{
    public FilterEmploymentStatusByNameRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Search criteria is required for filtering.");
    }
}
