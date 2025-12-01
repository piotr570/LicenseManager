using LicenseManager.Licenses.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LicenseManager.Licenses.Infrastructure.BackgroundServices;

public class LicenseAssignmentCleanupOptions
{
    public int DaysInactive { get; set; } = 30;
    public int IntervalInMinutes { get; set; } = 60;
    public bool IsEnabled { get; set; } = true;
}

public class LicenseReservationCleanupOptions
{
    public int DaysReserved { get; set; } = 7;
    public int IntervalInMinutes { get; set; } = 60;
    public bool IsEnabled { get; set; } = true;
}

public sealed class LicenseAssignmentCleanupBackgroundService(
    ILogger<LicenseAssignmentCleanupBackgroundService> logger,
    IServiceProvider serviceProvider,
    IOptions<LicenseAssignmentCleanupOptions> options)
    : BackgroundService
{
    private readonly LicenseAssignmentCleanupOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.IsEnabled)
        {
            logger.LogInformation("LicenseAssignmentCleanupBackgroundService is disabled");
            return;
        }

        logger.LogInformation(
            "LicenseAssignmentCleanupBackgroundService started. Interval: {IntervalMinutes} minutes, DaysInactive: {DaysInactive}",
            _options.IntervalInMinutes,
            _options.DaysInactive);

        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteCleanupAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(_options.IntervalInMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("LicenseAssignmentCleanupBackgroundService is shutting down");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in LicenseAssignmentCleanupBackgroundService");
                
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task ExecuteCleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<ILicenseAssignmentCleanupService>();
            
        logger.LogInformation("Starting license assignment cleanup process");
            
        var startTime = DateTime.UtcNow;
        var removedCount = await cleanupService.CleanupUnusedAssignmentsAsync(cancellationToken);
        var duration = DateTime.UtcNow - startTime;
            
        logger.LogInformation(
            "License assignment cleanup completed. Removed: {RemovedCount} assignments in {DurationMs}ms",
            removedCount,
            duration.TotalMilliseconds);
    }
}

public sealed class LicenseReservationCleanupBackgroundService(
    ILogger<LicenseReservationCleanupBackgroundService> logger,
    IServiceProvider serviceProvider,
    IOptions<LicenseReservationCleanupOptions> options)
    : BackgroundService
{
    private readonly LicenseReservationCleanupOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.IsEnabled)
        {
            logger.LogInformation("LicenseReservationCleanupBackgroundService is disabled");
            return;
        }

        logger.LogInformation(
            "LicenseReservationCleanupBackgroundService started. Interval: {IntervalMinutes} minutes, DaysReserved: {DaysReserved}",
            _options.IntervalInMinutes,
            _options.DaysReserved);

        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteCleanupAsync(stoppingToken);
                
                await Task.Delay(TimeSpan.FromMinutes(_options.IntervalInMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("LicenseReservationCleanupBackgroundService is shutting down");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred in LicenseReservationCleanupBackgroundService");
                
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task ExecuteCleanupAsync(CancellationToken cancellationToken)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var cleanupService = scope.ServiceProvider.GetRequiredService<ILicenseReservationCleanupService>();
            
            logger.LogInformation("Starting license reservation cleanup process");
            
            var startTime = DateTime.UtcNow;
            var removedCount = await cleanupService.CleanupExpiredReservationsAsync(cancellationToken);
            var duration = DateTime.UtcNow - startTime;
            
            logger.LogInformation(
                "License reservation cleanup completed. Removed: {RemovedCount} reservations in {DurationMs}ms",
                removedCount,
                duration.TotalMilliseconds);
        }
    }
}