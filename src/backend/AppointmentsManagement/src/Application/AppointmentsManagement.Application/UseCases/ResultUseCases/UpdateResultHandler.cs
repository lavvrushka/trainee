using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.DTOs;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.ResultUseCases;
public record UpdateResultRequest(
    Guid Id,
    string Complaints,
    string Conclusion,
    string Recommendations,
    Guid AppointmentId
) : IRequest<Unit>;
public class UpdateResultHandler : IRequestHandler<UpdateResultRequest, Unit>
{
    private readonly IResultRepository _repository;

    public UpdateResultHandler(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateResultRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException($"Result with Id {request.Id} not found.");

        request.MapToEntity(entity);

        _repository.Update(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
