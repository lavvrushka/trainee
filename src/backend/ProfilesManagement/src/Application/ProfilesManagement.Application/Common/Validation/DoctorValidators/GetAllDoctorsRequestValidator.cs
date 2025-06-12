using FluentValidation;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
namespace ProfilesManagement.Application.Common.Validation.DoctorValidators;

public class GetAllDoctorsRequestValidator : AbstractValidator<GetAllDoctorsRequest>
{
    public GetAllDoctorsRequestValidator()
    {
        RuleFor(r => r.PageIndex)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageIndex must be at least 1.");

        RuleFor(r => r.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("PageSize must not exceed 100.");
    }
}