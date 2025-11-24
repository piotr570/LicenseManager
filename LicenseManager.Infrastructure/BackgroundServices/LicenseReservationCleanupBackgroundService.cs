using LicenseManager.Application.Abstraction;
using LicenseManager.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LicenseManager.Infrastructure.BackgroundServices;

public class LicenseReservationCleanupBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<LicenseReservationCleanupOptions> options,
    ILogger<LicenseReservationCleanupBackgroundService> logger)
    : BackgroundService
{
    private readonly LicenseReservationCleanupOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "License Reservation Cleanup Background Service started. Cleanup frequency: {Frequency} hours",
            _options.CleanupFrequencyInHours);

        // Initial delay before first execution (prevents startup load)
        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("License Reservation Cleanup Background Service is stopping gracefully.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in License Reservation Cleanup Background Service. Will retry on next scheduled run.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromHours(_options.CleanupFrequencyInHours), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("License Reservation Cleanup Background Service stopped during delay.");
                break;
            }
        }

        logger.LogInformation("License Reservation Cleanup Background Service stopped.");
    }

    private async Task PerformCleanupAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating service scope for license reservation cleanup.");
        
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<ILicenseReservationCleanupService>();

        await cleanupService.CleanupExpiredReservationsAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("License Reservation Cleanup Background Service is stopping...");
        await base.StopAsync(cancellationToken);
    }
}