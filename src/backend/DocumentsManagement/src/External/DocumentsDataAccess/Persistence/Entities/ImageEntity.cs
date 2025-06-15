namespace DocumentsDataAccess.Persistence.Entities
{
    public class ImageEntity
    {
        public Guid Id { get; set; }
        public string BlobUrl { get; set; } = null!;
        public DateTime LastRetrievedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string ImageData { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
    }
}
