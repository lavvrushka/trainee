using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetUpcomingAppointmentsRequest(DateTime From, DateTime To) : IRequest<List<AppointmentDto>>;

public class GetUpcomingAppointmentsHandler : IRequestHandler<GetUpcomingAppointmentsRequest, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetUpcomingAppointmentsHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetUpcomingAppointmentsRequest request, CancellationToken cancellationToken)
    {
        var upcomingAppointments = await _appointmentRepository
            .GetUpcomingAsync(request.From, request.To, cancellationToken);

        var upcomingDtos = new List<AppointmentDto>();
        foreach (var appointment in upcomingAppointments)
        {
            upcomingDtos.Add(appointment.MapToDto());
        }

        return upcomingDtos;
    }
}
