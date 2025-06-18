using Azure.Storage.Blobs;
using DocumentsBusinessLogic.DTOs.Documents;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.DocumentsUseCases;

public record CreateDocumentRequest(
    IFormFile File
) : IRequest<Guid>;


public class CreateDocumentHandler : IRequestHandler<CreateDocumentRequest, Guid>
{
    private readonly IDocumentRepository _repository;
    private readonly BlobServiceClient _blobService;
    private readonly string _containerName;

    public CreateDocumentHandler(IDocumentRepository repository, BlobServiceClient blobService, IConfiguration config)
    {
        _repository = repository;
        _blobService = blobService;
        IOptions<AzureBlobSettings> options;
    }

    public async Task<Guid> Handle(CreateDocumentRequest request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var blobName = id.ToString();

        var container = _blobService.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(blobName);

        await using var stream = request.File.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: false, cancellationToken: ct);

        var blobUri = blobClient.Uri.ToString();

        var entity = request.ToEntity(blobUri);
        await _repository.AddAsync(entity);

        return entity.Id;
    }
}

