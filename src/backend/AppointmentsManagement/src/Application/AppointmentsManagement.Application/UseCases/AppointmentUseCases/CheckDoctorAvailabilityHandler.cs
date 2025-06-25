using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record CheckDoctorAvailabilityRequest(Guid DoctorId, DateTime Date) : IRequest<bool>;

public class CheckDoctorAvailabilityHandler : IRequestHandler<CheckDoctorAvailabilityRequest, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public CheckDoctorAvailabilityHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<bool> Handle(CheckDoctorAvailabilityRequest request, CancellationToken cancellationToken)
    {
        bool isTaken = await _appointmentRepository.ExistsByDoctorAndDateAsync(request.DoctorId, request.Date, cancellationToken);

        return isTaken;
    }
}
