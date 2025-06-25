using AppointmentsManagement.Application.UseCases.AppointmentUseCases;
using FastEndpoints;
using MyMediator.Interfaces;

namespace AppointmentsManagement.API.Endpoints.AppointmentsEndpoints;

public class CheckDoctorAvailabilityEndpoint : Endpoint<CheckDoctorAvailabilityRequest, bool>
{
    private readonly IMediator _mediator;

    public CheckDoctorAvailabilityEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/appointments/check-availability/{doctorId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckDoctorAvailabilityRequest request, CancellationToken ct)
    {
        bool isBusy = await _mediator.Send(request, ct);
        await SendOkAsync(isBusy, ct);
    }
}
