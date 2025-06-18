namespace BackgroundJobs.Options;

public class CleanupOptions
{
    public int PurgeIntervalMinutes { get; set; } = 60;
    public int HardDeleteIntervalMinutes { get; set; } = 120;
    public int AgeThresholdDays { get; set; } = 182;
    public string BlobContainerName { get; set; } = null!;
}