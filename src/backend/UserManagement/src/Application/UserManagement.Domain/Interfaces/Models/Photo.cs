namespace UserManagement.Domain.Interfaces.Models
{
    public class Photo:BaseModel
    {
        public string ImageData { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
    }
}
