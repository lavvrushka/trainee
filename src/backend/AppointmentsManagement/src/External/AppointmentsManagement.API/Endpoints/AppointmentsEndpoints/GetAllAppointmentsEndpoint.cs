using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using AppointmentsManagement.Domain.Models;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class GetAllAppointmentsEndpoint: Endpoint<GetAllAppointmentsRequest, Pagination<AppointmentDto>>
{
    private readonly IMediator _mediator;

    public GetAllAppointmentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllAppointmentsRequest request, CancellationToken ct)
    {
        Pagination<AppointmentDto> pagedResult =await _mediator.Send(request, ct);

        await SendOkAsync(pagedResult, ct);
    }
}
