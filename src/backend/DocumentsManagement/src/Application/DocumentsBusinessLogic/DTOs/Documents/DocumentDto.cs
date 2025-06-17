using DocumentsDataAccess.Persistence.Entities;
namespace DocumentsBusinessLogic.DTOs.Documents;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string BlobUrl { get; set; } = null!;
    public DateTime LastRetrievedAt { get; set; }
    public bool IsDeleted { get; set; }
}

public static class DocumentMapper
{
    public static DocumentDto ToDto(this DocumentEntity documentEntity)
    {
        var dto = new DocumentDto
        {
            Id = documentEntity.Id,
            BlobUrl = documentEntity.BlobUrl,
            LastRetrievedAt = documentEntity.LastRetrievedAt
        };

        return dto;
    }

    public static DocumentEntity ToEntity(this CreateDocumentRequest createDocumentRequest)
    {
        var entity = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            BlobUrl = createDocumentRequest.BlobUrl,
            LastRetrievedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        return entity;
    }

    public static void ApplyUpdate(this UpdateDocumentRequest updateDocumentRequest, DocumentEntity documentEntity)
    {
        documentEntity.BlobUrl = updateDocumentRequest.BlobUrl;
    }
}
