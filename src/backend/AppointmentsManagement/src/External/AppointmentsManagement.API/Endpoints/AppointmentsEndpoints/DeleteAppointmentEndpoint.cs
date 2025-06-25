using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class DeleteAppointmentEndpoint : Endpoint<DeleteAppointmentRequest>
{
    private readonly IMediator _mediator;

    public DeleteAppointmentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/appointments/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteAppointmentRequest request, CancellationToken ct)
    {
        await _mediator.Send(request, ct);
        await SendNoContentAsync(ct);
    }
}
