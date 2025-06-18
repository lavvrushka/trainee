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

    public CreateImageHandler( IImageRepository repository, BlobServiceClient blobService, IConfiguration config)
    {
        _repository = repository;
        _blobService = blobService;
        IOptions<AzureBlobSettings> options;
    }

    public async Task<Guid> Handle(CreateImageRequest request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var blobName = id.ToString();

        var containerClient = _blobService.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await using var stream = request.File.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: false, cancellationToken: ct);

        var blobUrl = blobClient.Uri.ToString();
        var entity = request.ToEntity(blobUrl);

        await _repository.AddAsync(entity);
        return entity.Id;
    }
}
