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
    private readonly IAppointmentRepository _repository;

    public UpdateAppointmentHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (appointment == null)
            return false;

        request.MapToEntity(appointment);

        _repository.Update(appointment);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
