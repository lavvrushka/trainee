using MyMediator.Interfaces;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
namespace ProfilesManagement.Application.UseCases.SpecializationUseCases;

public record DeleteSpecializationRequest(Guid Id) : IRequest<Unit>;
public class DeleteSpecializationHandler: IRequestHandler<DeleteSpecializationRequest, Unit>
{
    private readonly ISpecializationRepository _repository;

    public DeleteSpecializationHandler(ISpecializationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteSpecializationRequest req, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(req.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Specialization {req.Id} not found");
        }
        await _repository.DeleteAsync(entity);

        return Unit.Value;
    }
}
