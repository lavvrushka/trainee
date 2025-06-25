using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record UpdateAppointmentRequest(
Guid Id,
Guid PatientId,
Guid DoctorId,
Guid ServiceId,
DateTime Date,
bool IsApproved
) : IRequest<bool>;

public class UpdateAppointmentHandler : IRequestHandler<UpdateAppointmentRequest, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public UpdateAppointmentHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<bool> Handle(UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (appointment == null)
            return false;

        request.MapToEntity(appointment);

        _appointmentRepository.Update(appointment);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
