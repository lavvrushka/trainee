using FluentValidation;
using OfficesManagement.Core.DTOs.Requests;

namespace OfficesManagement.Core.Common.Validation;

public class LocationRequestValidator : AbstractValidator<LocationRequest>
{
    public LocationRequestValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");
    }
}