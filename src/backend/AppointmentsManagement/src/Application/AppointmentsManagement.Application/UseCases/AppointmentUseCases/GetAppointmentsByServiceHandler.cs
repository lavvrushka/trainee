using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetAppointmentsByServiceRequest(Guid ServiceId) : IRequest<List<AppointmentDto>>;

public class GetAppointmentsByServiceHandler : IRequestHandler<GetAppointmentsByServiceRequest, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByServiceHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByServiceRequest request, CancellationToken cancellationToken)
    {
        var serviceAppointments = await _appointmentRepository
            .GetByServiceAsync(request.ServiceId, cancellationToken);

        var serviceDtos = new List<AppointmentDto>();
        foreach (var appointment in serviceAppointments)
        {
            serviceDtos.Add(appointment.MapToDto());
        }

        return serviceDtos;
    }
}
