using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class CreateResultEndpoint : Endpoint<CreateResultRequest, ResultDto>
{
    private readonly IMediator _mediator;

    public CreateResultEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/results");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateResultRequest request, CancellationToken ct)
    {
        Guid createdResultId = await _mediator.Send(request, ct);

        await SendCreatedAtAsync<GetResultByIdEndpoint>(routeValues: new { id = createdResultId },cancellation: ct);
    }
}
