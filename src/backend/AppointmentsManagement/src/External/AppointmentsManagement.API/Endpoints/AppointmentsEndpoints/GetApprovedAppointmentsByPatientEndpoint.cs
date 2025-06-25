using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetApprovedAppointmentsByPatientEndpoint : Endpoint<GetApprovedAppointmentsByPatientRequest, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetApprovedAppointmentsByPatientEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/patient/{patientId:guid}/approved");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetApprovedAppointmentsByPatientRequest request, CancellationToken ct)
    {
        List<AppointmentDto> results = await _mediator.Send(request, ct);

        await SendOkAsync(results, ct);
    }
}
