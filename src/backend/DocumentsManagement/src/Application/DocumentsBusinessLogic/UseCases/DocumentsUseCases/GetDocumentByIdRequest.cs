using DocumentsBusinessLogic.DTOs.Documents;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.DocumentsUseCases;

public record GetDocumentByIdRequest(
     Guid Id
 ) : IRequest<DocumentDto>;

public class GetDocumentByIdHandler : IRequestHandler<GetDocumentByIdRequest, DocumentDto>
{
    private readonly IDocumentRepository _repository;

    public GetDocumentByIdHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }
    public async Task<DocumentDto> Handle(GetDocumentByIdRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        
        if (request == null)
        {
            throw new KeyNotFoundException($"Document with ID {request.Id} was not found.");
        }

        if (entity.IsDeleted)
        {
            throw new InvalidOperationException($"Document {entity.Id} is marked as deleted.");
        }

        return entity.ToDto();
    }
}
