using FluentValidation;
using ProfilesManagement.Application.UseCases.DoctorUseCases;
using System;

namespace ProfilesManagement.Application.Common.Validation.DoctorValidators
{
    public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
    {
        public CreateDoctorRequestValidator()
        {
            RuleFor(r => r.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("FirstName is required.")
                .Length(1, 100).WithMessage("FirstName must be between 1 and 100 characters.");

            RuleFor(r => r.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("LastName is required.")
                .Length(1, 100).WithMessage("LastName must be between 1 and 100 characters.");

            RuleFor(r => r.MiddleName)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("MiddleName must be provided (empty string if none).")
                .MaximumLength(100).WithMessage("MiddleName must not exceed 100 characters.");

            RuleFor(r => r.AccountId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("AccountId is required.");

            RuleFor(r => r.CareerStartYear)
                .Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage(r => $"CareerStartYear ({r.CareerStartYear:yyyy-MM-dd}) cannot be in the future.")
                .GreaterThanOrEqualTo(new DateTime(1930, 1, 1))
                .WithMessage("CareerStartYear must be no earlier than January 1, 1930.");

            RuleFor(r => r.StatusId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("StatusId is required.")
                .Must(id => id != Guid.Empty)
                .WithMessage("StatusId must be a non-empty GUID.");

            RuleFor(r => r.OfficeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("OfficeId is required.")
                .Must(id => id != Guid.Empty)
                .WithMessage("OfficeId must be a non-empty GUID.");

            RuleFor(r => r.SpecializationId)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("SpecializationId is required.")
                    .Must(id => id != Guid.Empty).WithMessage("SpecializationId must be a non-empty GUID.");

            RuleFor(r => r.ImageId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("ImageId is required.")
                .Must(id => id != Guid.Empty).WithMessage("ImageId must be a non-empty GUID.");
        }
    }
}
