using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetAppointmentByIdRequest(Guid Id) : IRequest<AppointmentDto?>;

public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdRequest, AppointmentDto?>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentByIdHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdRequest request, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(request.Id, cancellationToken);

        return appointment?.MapToDto();
    }
}
