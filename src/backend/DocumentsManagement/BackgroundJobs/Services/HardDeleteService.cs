using Azure.Storage.Blobs;
using BackgroundJobs.Options;
using DocumentsDataAccess.Persistence.Context;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace BackgroundJobs.Services;

public class HardDeleteService : BackgroundService
{
    private readonly ILogger<HardDeleteService> _logger;
    private readonly IServiceProvider _provider;
    private readonly TimeSpan _interval;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public HardDeleteService(IServiceProvider provider, IOptions<CleanupOptions> options, ILogger<HardDeleteService> logger, BlobServiceClient blobServiceClient)
    {
        _provider = provider;
        _logger = logger;
        _interval = TimeSpan.FromMinutes(options.Value.HardDeleteIntervalMinutes);
        _blobServiceClient = blobServiceClient;
        _containerName = options.Value.BlobContainerName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("HardDeleteService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var docRepo = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                var imgRepo = scope.ServiceProvider.GetRequiredService<IImageRepository>();

                var toDeleteDocs = await docRepo.ListMarkedDeletedAsync(DateTime.MaxValue);

                foreach (var doc in toDeleteDocs)
                {
                    var container = _blobServiceClient.GetBlobContainerClient(_containerName);
                    await container.GetBlobClient(doc.Id.ToString()).DeleteIfExistsAsync();
                    docRepo.Delete(doc);
                }

                var toDeleteImgs = await imgRepo.ListMarkedDeletedAsync(DateTime.MaxValue);
                foreach (var img in toDeleteImgs)
                {
                    var container = _blobServiceClient.GetBlobContainerClient(_containerName);
                    await container.GetBlobClient(img.Id.ToString()).DeleteIfExistsAsync();
                    imgRepo.Delete(img);
                }

                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("HardDeleteService: hard-deleted {docs} docs and {imgs} images",
                    toDeleteDocs.Count, toDeleteImgs.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HardDeleteService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
