using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class DeleteResultEndpoint : Endpoint<DeleteResultRequest>
{
    private readonly IMediator _mediator;

    public DeleteResultEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/results/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteResultRequest request, CancellationToken ct)
    {
        await _mediator.Send(request, ct);
        await SendNoContentAsync(ct);
    }
}
