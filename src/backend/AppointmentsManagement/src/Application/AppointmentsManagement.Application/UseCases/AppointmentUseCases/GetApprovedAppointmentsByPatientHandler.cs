using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetApprovedAppointmentsByPatientRequest(Guid PatientId) : IRequest<List<AppointmentDto>>;

public class GetApprovedAppointmentsByPatientHandler : IRequestHandler<GetApprovedAppointmentsByPatientRequest, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetApprovedAppointmentsByPatientHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<AppointmentDto>> Handle(GetApprovedAppointmentsByPatientRequest request, CancellationToken cancellationToken)
    {
        var approvedAppointments = await _appointmentRepository
            .GetApprovedByPatientAsync(request.PatientId, cancellationToken);

        var approvedDtos = new List<AppointmentDto>();
        foreach (var appointment in approvedAppointments)
        {
            approvedDtos.Add(appointment.MapToDto());
        }

        return approvedDtos;
    }
}
