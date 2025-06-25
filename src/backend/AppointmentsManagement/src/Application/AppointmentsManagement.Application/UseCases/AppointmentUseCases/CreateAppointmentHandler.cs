using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record CreateAppointmentRequest(
 Guid PatientId,
 Guid DoctorId,
 Guid ServiceId,
 DateTime Date
) : IRequest<Guid>;

public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentRequest, Guid>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CreateAppointmentHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }
    public async Task<Guid> Handle(CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = request.MapToEntity();

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}
