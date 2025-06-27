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
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public UpdateDocumentHandler(
        IDocumentRepository repository,
        BlobServiceClient blobServiceClient,
        IOptions<AzureBlobSettings> options)
    {
        _repository = repository;
        _blobServiceClient = blobServiceClient;
        _containerName = options.Value.ContainerName;
    }

    public async Task<Unit> Handle(UpdateDocumentRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Document with ID {request.Id} was not found.");
        }

        if (entity.IsDeleted)
        {
            throw new InvalidOperationException($"Cannot update document {entity.Id} because it is marked as deleted.");
        }

        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(request.Id.ToString());

        await using var stream = request.File.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);

        entity.BlobUrl = blobClient.Uri.ToString();

         _repository.Update(entity);

        return Unit.Value;
    }
}