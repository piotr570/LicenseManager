using System.Collections.Concurrent;
using LicenseManager.Observability.Metrics;

namespace LicenseManager.Observability;

public class CleanupMetricsCollector
{
    private readonly ConcurrentQueue<CleanupServiceMetrics> _metricsHistory = new();
    private const int MaxHistorySize = 100;

    public void RecordMetrics(CleanupServiceMetrics metrics)
    {
        _metricsHistory.Enqueue(metrics);

        while (_metricsHistory.Count > MaxHistorySize)
        {
            _metricsHistory.TryDequeue(out _);
        }
    }

    public IReadOnlyList<CleanupServiceMetrics> GetAllMetrics()
    {
        return _metricsHistory.ToList();
    }

    public IReadOnlyList<CleanupServiceMetrics> GetMetricsByService(string serviceName)
    {
        return _metricsHistory
            .Where(x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public CleanupServiceMetrics? GetLatestMetrics(string serviceName)
    {
        return _metricsHistory
            .Where(x => x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.StartTime)
            .FirstOrDefault();
    }

    public CleanupServiceStatistics GetStatistics(string serviceName)
    {
        var metrics = GetMetricsByService(serviceName);
        
        if (metrics.Count == 0)
        {
            return new CleanupServiceStatistics
            {
                ServiceName = serviceName,
                TotalRuns = 0,
                SuccessfulRuns = 0,
                FailedRuns = 0
            };
        }

        var successfulRuns = metrics.Where(x => x.IsSuccess).ToList();
        var failedRuns = metrics.Where(m => !m.IsSuccess).ToList();

        return new CleanupServiceStatistics
        {
            ServiceName = serviceName,
            TotalRuns = metrics.Count,
            SuccessfulRuns = successfulRuns.Count,
            FailedRuns = failedRuns.Count,
            TotalItemsProcessed = metrics.Sum(x => x.ItemsProcessed),
            TotalItemsCleaned = metrics.Sum(x => x.ItemsCleaned),
            AverageDurationMs = successfulRuns.Any() 
                ? successfulRuns.Average(x => x.Duration.TotalMilliseconds) 
                : 0,
            MinDurationMs = successfulRuns.Any() 
                ? successfulRuns.Min(x => x.Duration.TotalMilliseconds) 
                : 0,
            MaxDurationMs = successfulRuns.Any() 
                ? successfulRuns.Max(x => x.Duration.TotalMilliseconds) 
                : 0,
            LastRun = metrics.Max(x => x.StartTime),
            LastSuccessfulRun = successfulRuns.Any() 
                ? successfulRuns.Max(x => x.StartTime) 
                : null,
            LastFailure = failedRuns.Any() 
                ? failedRuns.Max(x => x.StartTime) 
                : null,
            RecentErrors = failedRuns
                .OrderByDescending(x => x.StartTime)
                .Take(5)
                .Select(m => new ErrorInfo
                {
                    Timestamp = m.StartTime,
                    Message = m.ErrorMessage ?? "Unknown error"
                })
                .ToList()
        };
    }

    public Dictionary<string, CleanupServiceStatistics> GetAllStatistics()
    {
        var serviceNames = _metricsHistory
            .Select(x => x.ServiceName)
            .Distinct()
            .ToList();

        return serviceNames.ToDictionary(
            name => name,
            name => GetStatistics(name)
        );
    }
}