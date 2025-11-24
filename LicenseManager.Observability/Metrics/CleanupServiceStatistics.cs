namespace LicenseManager.Observability.Metrics;

public class CleanupServiceStatistics
{
    public string ServiceName { get; init; } = string.Empty;
    public int TotalRuns { get; init; }
    public int SuccessfulRuns { get; init; }
    public int FailedRuns { get; init; }
    public int TotalItemsProcessed { get; init; }
    public int TotalItemsCleaned { get; init; }
    public double AverageDurationMs { get; init; }
    public double MinDurationMs { get; init; }
    public double MaxDurationMs { get; init; }
    public DateTime LastRun { get; init; }
    public DateTime? LastSuccessfulRun { get; init; }
    public DateTime? LastFailure { get; init; }
    public List<ErrorInfo> RecentErrors { get; init; } = new();
}

public class ErrorInfo
{
    public DateTime Timestamp { get; init; }
    public string Message { get; init; } = string.Empty;
}

