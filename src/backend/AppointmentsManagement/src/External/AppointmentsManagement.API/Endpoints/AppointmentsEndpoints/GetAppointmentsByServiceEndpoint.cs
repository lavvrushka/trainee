using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetAppointmentsByServiceEndpoint : Endpoint<GetAppointmentsByServiceRequest, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetAppointmentsByServiceEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/service/{serviceId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppointmentsByServiceRequest request, CancellationToken ct)
    {
        List<AppointmentDto> results = await _mediator.Send(request, ct);

        await SendOkAsync(results, ct);
    }
}
