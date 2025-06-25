using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record DeleteAppointmentRequest(Guid Id) : IRequest<bool>;

public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentRequest, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public DeleteAppointmentHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<bool> Handle(DeleteAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            return false;

        _appointmentRepository.Delete(appointment);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}