using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetAppointmentByIdEndpoint : Endpoint<GetAppointmentByIdRequest, AppointmentDto>
{
    private readonly IMediator _mediator;

    public GetAppointmentByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppointmentByIdRequest request, CancellationToken cancellationToken)
    {
        AppointmentDto? appointmentDto = await _mediator.Send(request, cancellationToken);

        await SendOkAsync(appointmentDto, cancellationToken);
    }
}
