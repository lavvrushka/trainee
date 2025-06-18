using DocumentsBusinessLogic.DTOs.Documents;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.DocumentsUseCases;

public record GetAllDocumentsRequest() : IRequest<IEnumerable<DocumentDto>>;

public class GetAllDocumentsHandler : IRequestHandler<GetAllDocumentsRequest, IEnumerable<DocumentDto>>
{
    private readonly IDocumentRepository _repository;

    public GetAllDocumentsHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DocumentDto>> Handle(GetAllDocumentsRequest request, CancellationToken ct)
    {
        var all = await _repository.GetAllAsync();

        return all.Where(e => !e.IsDeleted).Select(e => e.ToDto());
    }
}
