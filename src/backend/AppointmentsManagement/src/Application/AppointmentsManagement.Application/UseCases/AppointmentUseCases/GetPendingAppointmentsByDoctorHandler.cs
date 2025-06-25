using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetPendingAppointmentsByDoctorRequest(Guid DoctorId) : IRequest<List<AppointmentDto>>;

public class GetPendingAppointmentsByDoctorHandler : IRequestHandler<GetPendingAppointmentsByDoctorRequest, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetPendingAppointmentsByDoctorHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetPendingAppointmentsByDoctorRequest request, CancellationToken cancellationToken)
    {
        var pendingAppointments = await _appointmentRepository
            .GetPendingByDoctorAsync(request.DoctorId, cancellationToken);

        var pendingDtos = new List<AppointmentDto>();
        foreach (var appointment in pendingAppointments)
        {
            pendingDtos.Add(appointment.MapToDto());
        }

        return pendingDtos;
    }
}
