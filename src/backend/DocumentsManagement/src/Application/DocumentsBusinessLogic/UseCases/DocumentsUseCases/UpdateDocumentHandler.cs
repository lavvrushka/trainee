using Azure.Storage.Blobs;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.DocumentsUseCases;

public record UpdateDocumentRequest(
    Guid Id,
    IFormFile File
) : IRequest<Unit>;

public class UpdateDocumentHandler : IRequestHandler<UpdateDocumentRequest, Unit>
{
    private readonly IDocumentRepository _repository;
    private readonly BlobServiceClient _blobService;
    private readonly string _containerName;

    public UpdateDocumentHandler(
        IDocumentRepository repository,
        BlobServiceClient blobService,
        IOptions<AzureBlobSettings> options)
    {
        _repository = repository;
        _blobService = blobService;
        _containerName = options.Value.ContainerName;
    }

    public async Task<Unit> Handle(UpdateDocumentRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Document {request.Id} не найдена");
        }

        if (entity.IsDeleted)
        {
            throw new InvalidOperationException($"Document {request.Id} помечена как удалённая");
        }

        var container = _blobService.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(request.Id.ToString());

        await using var stream = request.File.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);

        entity.BlobUrl = blobClient.Uri.ToString();
        _repository.Update(entity);

        return Unit.Value;
    }
}
