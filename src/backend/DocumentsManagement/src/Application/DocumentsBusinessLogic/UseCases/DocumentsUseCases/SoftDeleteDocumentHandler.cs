using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.DocumentsUseCases;

public record SoftDeleteDocumentRequest(
     Guid Id
 ) : IRequest<Unit>;

public class SoftDeleteDocumentHandler : IRequestHandler<SoftDeleteDocumentRequest, Unit>
{
    private readonly IDocumentRepository _repository;

    public SoftDeleteDocumentHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(SoftDeleteDocumentRequest request, CancellationToken ct)
    {
        await _repository.SoftDeleteAsync(request.Id);

        return Unit.Value;
    }
}
