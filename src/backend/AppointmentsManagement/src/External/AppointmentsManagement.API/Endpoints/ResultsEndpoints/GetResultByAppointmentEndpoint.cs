using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class GetResultByAppointmentEndpoint : Endpoint<GetResultByAppointmentRequest, ResultDto>
{
    private readonly IMediator _mediator;

    public GetResultByAppointmentEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/results/by-appointment/{appointmentId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetResultByAppointmentRequest request, CancellationToken ct)
    {
        ResultDto? dto = await _mediator.Send(request, ct);

        await SendOkAsync(dto, ct);
    }
}
