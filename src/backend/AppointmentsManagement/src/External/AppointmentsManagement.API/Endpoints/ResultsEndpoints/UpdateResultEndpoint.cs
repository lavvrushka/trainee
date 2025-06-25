using AppointmentsManagement.Application.UseCases.ResultUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.ResultsEndpoints;

public class UpdateResultEndpoint : Endpoint<UpdateResultRequest>
{
    private readonly IMediator _mediator;

    public UpdateResultEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/results/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateResultRequest request, CancellationToken ct)
    {
        await _mediator.Send(request, ct);
        await SendNoContentAsync(ct);
    }
}
