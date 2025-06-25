using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetPendingAppointmentsByDoctorEndpoint : Endpoint<GetPendingAppointmentsByDoctorRequest, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetPendingAppointmentsByDoctorEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/doctor/{doctorId:guid}/pending");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetPendingAppointmentsByDoctorRequest request, CancellationToken ct)
    {
        List<AppointmentDto> results = await _mediator.Send(request, ct);

        await SendOkAsync(results, ct);
    }
}
