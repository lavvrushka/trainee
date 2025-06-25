using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class CreateAppointmentEndpoint : Endpoint<CreateAppointmentRequest, AppointmentDto>
{
    private readonly IMediator _mediator;

    public CreateAppointmentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/appointments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        Guid createdAppointmentId = await _mediator.Send(request, cancellationToken);

        await SendCreatedAtAsync<GetAppointmentByIdEndpoint>(
            routeValues: new { id = createdAppointmentId },
            cancellation: cancellationToken);
    }
}