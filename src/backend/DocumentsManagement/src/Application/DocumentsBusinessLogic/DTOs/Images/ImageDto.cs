using DocumentsBusinessLogic.UseCases.ImagesUseCases;
using DocumentsDataAccess.Persistence.Entities;
namespace DocumentsBusinessLogic.DTOs.Images;

public class ImageDto
{
    public Guid Id { get; set; }
    public string BlobUrl { get; set; } = null!;
    public DateTime LastRetrievedAt { get; set; }
    public bool IsDeleted { get; set; }
}

public static class ImageMapper
{
    public static ImageDto ToDto(this ImageEntity imageEntity)
    {
        var dto = new ImageDto
        {
            Id = imageEntity.Id,
            BlobUrl = imageEntity.BlobUrl,
            LastRetrievedAt = imageEntity.LastRetrievedAt,
            IsDeleted = imageEntity.IsDeleted
        };

        return dto;
    }

    public static ImageEntity ToEntity(this CreateImageRequest createImageRequest, string blobUri)
    {
        var entity = new ImageEntity
        {
            Id = Guid.NewGuid(),
            BlobUrl = blobUri,
            LastRetrievedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        return entity;
    }

    public static void ApplyUpdate(this UpdateImageRequest updateImageRequest, ImageEntity imageEntity, string blobUri)
    {
        imageEntity.BlobUrl = blobUri;
    }

}
