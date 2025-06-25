using AppointmentsManagement.Application.DTOs;
using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class GetResultByIdEndpoint : Endpoint<GetResultByIdRequest, ResultDto>
{
    private readonly IMediator _mediator;

    public GetResultByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/results/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetResultByIdRequest request, CancellationToken ct)
    {
        ResultDto? dto = await _mediator.Send(request, ct);

        await SendOkAsync(dto, ct);
    }
}
