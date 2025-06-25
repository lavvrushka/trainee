using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class UpdateAppointmentEndpoint : Endpoint<UpdateAppointmentRequest>
{
    private readonly IMediator _mediator;

    public UpdateAppointmentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/appointments/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateAppointmentRequest request, CancellationToken ct)
    {
        await _mediator.Send(request, ct);
        await SendNoContentAsync(ct);
    }
}
