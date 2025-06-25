using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetAppointmentsByDoctorAndDateRequest(Guid DoctorId, DateTime Date) : IRequest<List<AppointmentDto>>;

public class GetAppointmentsByDoctorAndDateHandler : IRequestHandler<GetAppointmentsByDoctorAndDateRequest, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByDoctorAndDateHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByDoctorAndDateRequest request, CancellationToken cancellationToken)
    {
        var startOfDay = request.Date.Date;
        var nextDay = startOfDay.AddDays(1);

        var appointments = await _appointmentRepository .GetByDoctorAndDateAsync(request.DoctorId, request.Date, cancellationToken);

        var appointmentDtos = new List<AppointmentDto>();
        foreach (var appointment in appointments)
        {
            appointmentDtos.Add(appointment.MapToDto());
        }

        return appointmentDtos;
    }
}
