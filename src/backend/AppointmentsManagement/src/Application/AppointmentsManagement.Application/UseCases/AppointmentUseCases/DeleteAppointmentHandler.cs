using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.AppointmentUseCases;

public record DeleteAppointmentRequest(Guid Id) : IRequest<bool>;

public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentRequest, bool>
{
    private readonly IAppointmentRepository _repository;

    public DeleteAppointmentHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment == null)
            return false;

        _repository.Delete(appointment);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}