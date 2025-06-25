using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FluentValidation;

namespace AppointmentsManagement.Application.Common.Validation.ResultValidators;

public class UpdateResultRequestValidator : AbstractValidator<UpdateResultRequest>
{
    public UpdateResultRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Result Id is required.");

        RuleFor(x => x.Complaints)
            .MaximumLength(1000).WithMessage("Complaints must be at most 1000 characters.");

        RuleFor(x => x.Conclusion)
            .MaximumLength(1000).WithMessage("Conclusion must be at most 1000 characters.");

        RuleFor(x => x.Recommendations)
            .MaximumLength(1000).WithMessage("Recommendations must be at most 1000 characters.");
    }
}
