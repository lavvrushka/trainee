using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FluentValidation;

namespace AppointmentsManagement.Application.Common.Validation.ResultValidators;

public class CreateResultRequestValidator : AbstractValidator<CreateResultRequest>
{
    public CreateResultRequestValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty().WithMessage("AppointmentId is required.");

        RuleFor(x => x.Complaints)
            .MaximumLength(1000).WithMessage("Complaints must be at most 1000 characters.");

        RuleFor(x => x.Conclusion)
            .MaximumLength(1000).WithMessage("Conclusion must be at most 1000 characters.");

        RuleFor(x => x.Recommendations)
            .MaximumLength(1000).WithMessage("Recommendations must be at most 1000 characters.");
    }
}
