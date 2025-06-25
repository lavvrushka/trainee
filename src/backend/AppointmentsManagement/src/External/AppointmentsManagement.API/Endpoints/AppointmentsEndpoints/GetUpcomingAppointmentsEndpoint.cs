using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetUpcomingAppointmentsEndpoint : Endpoint<GetUpcomingAppointmentsRequest, List<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetUpcomingAppointmentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/upcoming");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetUpcomingAppointmentsRequest request, CancellationToken ct)
    {
        List<AppointmentDto> results = await _mediator.Send(request, ct);

        await SendOkAsync(results, ct);
    }
}


