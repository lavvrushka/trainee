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
    private readonly IAppointmentRepository _repository;

    public CreateAppointmentHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = request.MapToEntity();

        await _repository.AddAsync(appointment, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}
