using FluentValidation;
using OfficesManagement.Core.UseCases;

namespace OfficesManagement.Core.Common.Validation;

public class CreateOfficeRequestValidator : AbstractValidator<CreateOfficeRequest>
{
    public CreateOfficeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.RegistryPhoneNumber)
            .NotEmpty().WithMessage("Registry phone number is required.")
            .MaximumLength(20).WithMessage("Registry phone number cannot exceed 20 characters.");

        RuleFor(x => x.Location)
            .NotNull().WithMessage("Location is required.")
            .SetValidator(new LocationRequestValidator());
    }
}

public class UpdateOfficeRequestValidator : AbstractValidator<UpdateOfficeRequest>
{
    public UpdateOfficeRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.RegistryPhoneNumber)
            .MaximumLength(20).WithMessage("Registry phone number cannot exceed 20 characters.");

        RuleFor(x => x.Location)
            .SetValidator(new LocationRequestValidator())
            .When(x => x.Location is not null);
    }
}