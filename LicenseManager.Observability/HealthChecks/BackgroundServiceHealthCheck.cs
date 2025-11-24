using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LicenseManager.Observability.HealthChecks;

public class BackgroundServiceHealthCheck : IHealthCheck
{
    private readonly Dictionary<string, ServiceHealthStatus> _serviceStatuses = new();
    private readonly object _lock = new();

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_serviceStatuses.Count == 0)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("No background services registered yet"));
            }

            var unhealthyServices = _serviceStatuses
                .Where(s => !s.Value.IsHealthy)
                .ToList();

            if (unhealthyServices.Count > 0)
            {
                var data = unhealthyServices.ToDictionary(
                    s => s.Key,
                    s => (object)new
                    {
                        s.Value.LastRun,
                        s.Value.LastError,
                        s.Value.ConsecutiveFailures
                    });

                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        $"{unhealthyServices.Count} background service(s) are unhealthy",
                        data: data));
            }

            var serviceData = _serviceStatuses.ToDictionary(
                s => s.Key,
                s => (object)new
                {
                    s.Value.LastRun,
                    s.Value.LastSuccess,
                    s.Value.RunCount
                });

            return Task.FromResult(
                HealthCheckResult.Healthy(
                    "All background services are healthy",
                    data: serviceData));
        }
    }

    public void RecordSuccess(string serviceName)
    {
        lock (_lock)
        {
            if (!_serviceStatuses.ContainsKey(serviceName))
            {
                _serviceStatuses[serviceName] = new ServiceHealthStatus();
            }

            var status = _serviceStatuses[serviceName];
            status.LastRun = DateTime.UtcNow;
            status.LastSuccess = DateTime.UtcNow;
            status.ConsecutiveFailures = 0;
            status.RunCount++;
            status.IsHealthy = true;
        }
    }

    public void RecordFailure(string serviceName, Exception exception)
    {
        lock (_lock)
        {
            if (!_serviceStatuses.ContainsKey(serviceName))
            {
                _serviceStatuses[serviceName] = new ServiceHealthStatus();
            }

            var status = _serviceStatuses[serviceName];
            status.LastRun = DateTime.UtcNow;
            status.LastError = exception.Message;
            status.ConsecutiveFailures++;
            status.IsHealthy = status.ConsecutiveFailures < 3; // Unhealthy after 3 consecutive failures
        }
    }

    private class ServiceHealthStatus
    {
        public DateTime? LastRun { get; set; }
        public DateTime? LastSuccess { get; set; }
        public string? LastError { get; set; }
        public int ConsecutiveFailures { get; set; }
        public int RunCount { get; set; }
        public bool IsHealthy { get; set; } = true;
    }
}

