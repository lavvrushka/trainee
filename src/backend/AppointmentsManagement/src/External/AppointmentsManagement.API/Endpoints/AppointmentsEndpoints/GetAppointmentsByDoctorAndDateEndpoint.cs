using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetAppointmentsByDoctorAndDateEndpoint : Endpoint<GetAppointmentsByDoctorAndDateRequest, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetAppointmentsByDoctorAndDateEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/doctor/{doctorId:guid}/by-date");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppointmentsByDoctorAndDateRequest request, CancellationToken ct)
    {
        List<AppointmentDto> results = await _mediator.Send(request, ct);

        await SendOkAsync(results, ct);
    }
}
