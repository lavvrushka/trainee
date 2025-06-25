using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.ResultUseCases;
using AppointmentsManagement.Domain.Models;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class GetAllResultsEndpoint : Endpoint<GetAllResultsRequest, Pagination<ResultDto>>
{
    private readonly IMediator _mediator;

    public GetAllResultsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/results");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllResultsRequest request, CancellationToken ct)
    {
        Pagination<ResultDto> paged = await _mediator.Send(request, ct);
        await SendOkAsync(paged, ct);
    }
}
