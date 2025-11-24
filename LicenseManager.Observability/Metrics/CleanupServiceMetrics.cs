namespace LicenseManager.Observability.Metrics;

public class CleanupServiceMetrics
{
    public string ServiceName { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; set; }
    public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;
    public int ItemsProcessed { get; set; }
    public int ItemsCleaned { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

