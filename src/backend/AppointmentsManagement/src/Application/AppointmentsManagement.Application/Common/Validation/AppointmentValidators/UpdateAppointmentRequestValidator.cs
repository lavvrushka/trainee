using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FluentValidation;

namespace AppointmentsManagement.Application.Common.Validation.AppointmentValidators;

public class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
{
    public UpdateAppointmentRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment Id is required.");

        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("PatientId is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("DoctorId is required.");

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("ServiceId is required.");

        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Date must be today or in the future.");
    }
}
