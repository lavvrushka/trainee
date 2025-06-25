using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Domain.Models;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetAllAppointmentsRequest(PageSettings PageSettings) : IRequest<Pagination<AppointmentDto>>;

public class GetAllAppointmentsHandler : IRequestHandler<GetAllAppointmentsRequest, Pagination<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAllAppointmentsHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Pagination<AppointmentDto>> Handle(GetAllAppointmentsRequest request, CancellationToken cancellationToken)
    {

        Pagination<Appointment> pagedAppointments = await _appointmentRepository.GetAllAsync(request.PageSettings, cancellationToken);

        List<AppointmentDto> dtos = pagedAppointments.Items.Select(appt => appt.MapToDto()).ToList();

        return new Pagination<AppointmentDto>( dtos, pagedAppointments.TotalCount, request.PageSettings);
    }
}
