using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record GetAppointmentByIdRequest(Guid Id) : IRequest<AppointmentDto?>;

public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdRequest, AppointmentDto?>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAppointmentByIdHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<AppointmentDto?> Handle(GetAppointmentByIdRequest request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id, cancellationToken);

        return appointment?.MapToDto();
    }
}
