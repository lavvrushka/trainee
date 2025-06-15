namespace DocumentsDataAccess.Persistence.Entities;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public string BlobUrl { get; set; } = null!;
    public DateTime LastRetrievedAt { get; set; }
    public bool IsDeleted { get; set; }
}
