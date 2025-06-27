using Azure.Storage.Blobs;
using DocumentsDataAccess.Persistence.Options;
using Microsoft.Extensions.Options;

namespace DocumentsAPI.Extensions;

public static class BlobServiceCollectionExtensions
{
    public static IServiceCollection AddBlobServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.Configure<AzureBlobSettings>(
            configuration.GetSection("AzureBlob"));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureBlobSettings>>().Value;
            return new BlobServiceClient(options.ConnectionString);
        });

        return services;
    }
}
