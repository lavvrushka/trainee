using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class GetRecentResultsEndpoint : Endpoint<GetRecentResultsRequest, List<ResultDto>>
{
    private readonly IMediator _mediator;

    public GetRecentResultsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/results/recent/{days:int}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetRecentResultsRequest request, CancellationToken ct)
    {
        List<ResultDto> dtos = await _mediator.Send(request, ct);
        await SendOkAsync(dtos, ct);
    }
}
