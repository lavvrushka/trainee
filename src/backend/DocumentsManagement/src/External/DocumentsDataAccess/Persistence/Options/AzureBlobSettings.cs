namespace DocumentsDataAccess.Persistence.Options;

public class AzureBlobSettings
{
    public string ConnectionString { get; set; } = default!;
    public string ContainerName { get; set; } = default!;
}
