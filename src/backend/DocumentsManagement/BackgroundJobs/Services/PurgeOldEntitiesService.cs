using BackgroundJobs.Options;
using DocumentsDataAccess.Persistence.Interfaces.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace BackgroundJobs.Services;

public class PurgeOldEntitiesService : BackgroundService
{
    private readonly ILogger<PurgeOldEntitiesService> _logger;
    private readonly IServiceProvider _provider;
    private readonly TimeSpan _interval;
    private readonly TimeSpan _ageThreshold;

    public PurgeOldEntitiesService(IServiceProvider provider, IOptions<CleanupOptions> options, ILogger<PurgeOldEntitiesService> logger)
    {
        _provider = provider;
        _logger = logger;
        _interval = TimeSpan.FromMinutes(options.Value.PurgeIntervalMinutes);
        _ageThreshold = TimeSpan.FromDays(options.Value.AgeThresholdDays);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PurgeOldEntitiesService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cutoff = DateTime.UtcNow.Subtract(_ageThreshold);

                using var scope = _provider.CreateScope();
                var docRepo = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                var imgRepo = scope.ServiceProvider.GetRequiredService<IImageRepository>();

                var oldDocs = await docRepo.ListMarkedDeletedAsync(cutoff);
                foreach (var doc in oldDocs)
                {
                    await docRepo.SoftDeleteAsync(doc.Id);
                }

                var oldImgs = await imgRepo.ListMarkedDeletedAsync(cutoff);
                foreach (var img in oldImgs)
                {
                    await imgRepo.SoftDeleteAsync(img.Id);
                }

                _logger.LogInformation("PurgeOldEntitiesService: Soft-deleted {docs} docs and {imgs} images older than {cutoff}",
                    oldDocs.Count, oldImgs.Count, cutoff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PurgeOldEntitiesService");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
