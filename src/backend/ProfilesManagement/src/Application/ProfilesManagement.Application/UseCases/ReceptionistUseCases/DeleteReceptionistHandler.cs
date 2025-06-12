using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
namespace ProfilesManagement.Application.UseCases.ReceptionistUseCases;

public record DeleteReceptionistRequest(Guid Id) : IRequest<Unit>;

public class DeleteReceptionistHandler : IRequestHandler<DeleteReceptionistRequest, Unit>
{
    private readonly IReceptionistRepository _repository;
    public DeleteReceptionistHandler(IReceptionistRepository repository)
    {
        _repository = repository;
    }
    public async Task<Unit> Handle(DeleteReceptionistRequest request,CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Receptionist {request.Id} not found");

        }
        await _repository.DeleteAsync(existing);

        return Unit.Value;
    }
}