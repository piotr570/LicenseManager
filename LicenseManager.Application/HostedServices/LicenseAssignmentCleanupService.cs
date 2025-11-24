using LicenseManager.Application.Abstraction;
using LicenseManager.Domain.Assignments;
using LicenseManager.Domain.Licenses;
using LicenseManager.Observability;
using LicenseManager.Observability.HealthChecks;
using LicenseManager.Observability.Metrics;
using LicenseManager.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.HostedServices;

public class LicenseAssignmentCleanupService(
    IReadDbContext readDbContext,
    IRepository<License> repository,
    IUnitOfWork unitOfWork,
    ILogger<LicenseAssignmentCleanupService> logger,
    BackgroundServiceHealthCheck? healthCheck = null,
    CleanupMetricsCollector? metricsCollector = null)
    : ILicenseAssignmentCleanupService
{
    private const string ServiceName = nameof(LicenseAssignmentCleanupService);

    public async Task CleanupNotUsedAssignmentsAsync(CancellationToken cancellationToken)
    {
        var metrics = new CleanupServiceMetrics
        {
            ServiceName = ServiceName,
            StartTime = DateTime.UtcNow
        };

        logger.LogInformation("Starting {ServiceName} service execution", ServiceName);

        try
        {
            var licensesWithNotUsedAssignments = await readDbContext
                .Set<License>()
                .Include(x => x.Assignments)
                .Where(x => x.Assignments.Any(a => a.State == AssignmentState.NotUsed))
                .ToListAsync(cancellationToken);

            metrics.ItemsProcessed = licensesWithNotUsedAssignments.Count;

            if (licensesWithNotUsedAssignments.Count == 0)
            {
                logger.LogInformation("No licenses with unused assignments found. Skipping cleanup.");
                metrics.IsSuccess = true;
                metricsCollector?.RecordMetrics(metrics);
                healthCheck?.RecordSuccess(ServiceName);
                return;
            }

            var totalCleaned = 0;

            foreach (var license in licensesWithNotUsedAssignments)
            {
                var cleanedCount = license.CleanupNotUsedAssignments(a => a.State == AssignmentState.NotUsed);

                if (cleanedCount > 0)
                {
                    logger.LogInformation(
                        "Cleaned up {CleanedCount} unused assignments for license {LicenseId} ({LicenseName})",
                        cleanedCount, license.Id, license.Name);
                    totalCleaned += cleanedCount;
                    repository.Update(license);
                }
            }

            if (totalCleaned > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation(
                    "Successfully cleaned up {TotalCleaned} unused assignments across {LicenseCount} licenses",
                    totalCleaned, licensesWithNotUsedAssignments.Count);
            }
            else
            {
                logger.LogInformation("No changes detected. Skipping database save operation.");
            }

            metrics.ItemsCleaned = totalCleaned;
            metrics.IsSuccess = true;
            metrics.EndTime = DateTime.UtcNow;

            logger.LogInformation(
                "{ServiceName} completed successfully. Processed: {Processed}, Cleaned: {Cleaned}, Duration: {Duration}ms",
                ServiceName, metrics.ItemsProcessed, metrics.ItemsCleaned, metrics.Duration.TotalMilliseconds);

            healthCheck?.RecordSuccess(ServiceName);
            metricsCollector?.RecordMetrics(metrics);
        }
        catch (Exception ex)
        {
            metrics.IsSuccess = false;
            metrics.ErrorMessage = ex.Message;
            metrics.EndTime = DateTime.UtcNow;

            logger.LogError(ex,
                "{ServiceName} failed after {Duration}ms. Processed: {Processed}",
                ServiceName, metrics.Duration.TotalMilliseconds, metrics.ItemsProcessed);

            healthCheck?.RecordFailure(ServiceName, ex);
            metricsCollector?.RecordMetrics(metrics);
            throw;
        }
    }
}