using Azure.Storage.Blobs;
using DocumentsBusinessLogic.DTOs.Documents;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Options;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.DocumentsUseCases;

public record DownloadDocumentRequest(Guid Id) : IRequest<DownloadDocumentResponse>;
public class DownloadDocumentResponse
{
    public Stream Stream { get; set; } = default!;
    public string ContentType { get; set; } = "application/octet-stream";
    public string FileName { get; set; } = "document";
}
public class DownloadDocumentHandler : IRequestHandler<DownloadDocumentRequest, DownloadDocumentResponse>
{
    private readonly IDocumentRepository _repository;
    private readonly BlobServiceClient _blobService;
    private readonly string _containerName;

    public DownloadDocumentHandler(
        IDocumentRepository repository,
        BlobServiceClient blobService,
        IOptions<AzureBlobSettings> options)
    {
        _repository = repository;
        _blobService = blobService;
        _containerName = options.Value.ContainerName;
    }

    public async Task<DownloadDocumentResponse> Handle(DownloadDocumentRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Документ {request.Id} не найден");

        }

        if (entity.IsDeleted)
        {
            throw new InvalidOperationException($"Документ {request.Id} помечен как удалённый");
        }
            
        var blobClient = _blobService
            .GetBlobContainerClient(_containerName)
            .GetBlobClient(request.Id.ToString());

        var blobResult = await blobClient.DownloadStreamingAsync(cancellationToken: ct);

        return entity.ToDownloadResponse(blobResult.Value);
    }
}
