using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetAppointmentsByPatientRequest(Guid PatientId) : IRequest<List<AppointmentDto>>;

public class GetAppointmentsByPatientHandler : IRequestHandler<GetAppointmentsByPatientRequest, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentsByPatientHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetAppointmentsByPatientRequest request, CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetByPatientAsync(request.PatientId, cancellationToken);

        var appointmentDtos = new List<AppointmentDto>();

        foreach (var appointment in appointments)
        {
            appointmentDtos.Add(appointment.MapToDto());
        }

        return appointmentDtos;
    }
}
