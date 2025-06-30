using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentsBusinessLogic.DTOs.Images;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;
namespace DocumentsBusinessLogic.UseCases.ImagesUseCases;

public record CreateImageRequest(
    IFormFile File
) : IRequest<Guid>;

public class CreateImageHandler : IRequestHandler<CreateImageRequest, Guid>
{
    private readonly IImageRepository _repository;
    private readonly BlobServiceClient _blobService;
    private readonly string _containerName;

    public CreateImageHandler(
        IImageRepository repository,
        BlobServiceClient blobService,
        IOptions<AzureBlobSettings> options)
    {
        _repository = repository;
        _blobService = blobService;
        _containerName = options.Value.ContainerName;
    }

    public async Task<Guid> Handle(CreateImageRequest request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var blobName = id.ToString();
        var container = _blobService.GetBlobContainerClient(_containerName);

        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = container.GetBlobClient(blobName);
        await using var stream = request.File.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: false, cancellationToken: ct);

        var blobUrl = blobClient.Uri.ToString();
        var entity = request.ToEntity(blobUrl);
        await _repository.AddAsync(entity);

        return entity.Id;
    }
}
