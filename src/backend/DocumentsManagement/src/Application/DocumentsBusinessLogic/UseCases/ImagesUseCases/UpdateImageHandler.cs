using Azure.Storage.Blobs;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using DocumentsDataAccess.Persistence.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MyMediator.Interfaces;

public record UpdateImageRequest(
    Guid Id,
    IFormFile File
) : IRequest<Unit>;

public class UpdateImageHandler : IRequestHandler<UpdateImageRequest, Unit>
{
    private readonly IImageRepository _repository;
    private readonly BlobServiceClient _blobService;
    private readonly string _containerName;

    public UpdateImageHandler(
        IImageRepository repository,
        BlobServiceClient blobService,
        IOptions<AzureBlobSettings> options)
    {
        _repository = repository;
        _blobService = blobService;
        _containerName = options.Value.ContainerName;
    }

    public async Task<Unit> Handle(UpdateImageRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(request.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Image {request.Id} не найдена");
        }

        if (entity.IsDeleted)
        {
            throw new InvalidOperationException($"Image {request.Id} помечена как удалённая");
        }

        var container = _blobService.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(request.Id.ToString());

        await blobClient.UploadAsync(request.File.OpenReadStream(), overwrite: true, cancellationToken: ct);

        entity.BlobUrl = blobClient.Uri.ToString();
        _repository.Update(entity);

        return Unit.Value;
    }
}
