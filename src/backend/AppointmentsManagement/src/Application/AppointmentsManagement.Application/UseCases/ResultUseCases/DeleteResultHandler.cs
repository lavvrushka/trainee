using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using MyMediator.Interfaces;

namespace AppointmentsManagement.Application.UseCases.ResultUseCases;
public record DeleteResultRequest(Guid Id) : IRequest<Unit>;
public class DeleteResultHandler : IRequestHandler<DeleteResultRequest, Unit>
{
    private readonly IResultRepository _repository;

    public DeleteResultHandler(IResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteResultRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException($"Result with Id {request.Id} not found.");

        _repository.Delete(entity);
        await _repository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
