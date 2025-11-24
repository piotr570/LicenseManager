using LicenseManager.Application.Abstraction;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Reservations;
using LicenseManager.Observability;
using LicenseManager.Observability.HealthChecks;
using LicenseManager.Observability.Metrics;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.HostedServices;

public class LicenseReservationCleanupService(
    IReadDbContext readDbContext,
    IRepository<License> repository,
    IUnitOfWork unitOfWork,
    ILogger<LicenseReservationCleanupService> logger,
    BackgroundServiceHealthCheck? healthCheck = null,
    CleanupMetricsCollector? metricsCollector = null)
    : ILicenseReservationCleanupService
{
    private const string ServiceName = nameof(LicenseReservationCleanupService);

    public async Task CleanupExpiredReservationsAsync(CancellationToken cancellationToken)
    {
        var metrics = new CleanupServiceMetrics
        {
            ServiceName = ServiceName,
            StartTime = DateTime.UtcNow
        };

        logger.LogInformation("Starting {ServiceName} service execution", ServiceName);

        try
        {
            var now = SystemClock.Now;

            var licensesWithReservations = await readDbContext
                .Set<License>()
                .Where(x => x.ReservationIds.Any())
                .ToListAsync(cancellationToken);

            metrics.ItemsProcessed = licensesWithReservations.Count;

            if (licensesWithReservations.Count == 0)
            {
                logger.LogInformation("No licenses with reservations found. Skipping cleanup.");
                metrics.IsSuccess = true;
                healthCheck?.RecordSuccess(ServiceName);
                return;
            }

            // Load reservations to check expiration
            var allReservationIds = licensesWithReservations
                .SelectMany(x => x.ReservationIds)
                .Distinct()
                .ToList();

            var expiredReservationIds = await readDbContext
                .Set<LicenseReservation>()
                .Where(x => allReservationIds.Contains(x.Id))
                .Where(x => x.ExpirationDate.HasValue && x.ExpirationDate.Value < now)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (expiredReservationIds.Count == 0)
            {
                logger.LogInformation("No expired reservations found. Skipping cleanup.");
                metrics.IsSuccess = true;
                healthCheck?.RecordSuccess(ServiceName);
                metricsCollector?.RecordMetrics(metrics);
                return;
            }

            var expiredSet = expiredReservationIds.ToHashSet();
            var totalCleaned = 0;

            foreach (var license in licensesWithReservations)
            {
                var cleanedCount = license.CleanupExpiredReservations(expiredSet.Contains);

                if (cleanedCount > 0)
                {
                    logger.LogInformation(
                        "Cleaned up {CleanedCount} expired reservations for license {LicenseId} ({LicenseName})",
                        cleanedCount, license.Id, license.Name);
                    totalCleaned += cleanedCount;
                    repository.Update(license);
                }
            }

            if (totalCleaned > 0)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation(
                    "Successfully cleaned up {TotalCleaned} expired reservations across {LicenseCount} licenses",
                    totalCleaned, licensesWithReservations.Count);
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