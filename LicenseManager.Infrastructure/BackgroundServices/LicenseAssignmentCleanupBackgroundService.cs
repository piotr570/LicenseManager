using LicenseManager.Application.Abstraction;
using LicenseManager.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LicenseManager.Infrastructure.BackgroundServices;

public class LicenseAssignmentCleanupBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<LicenseAssignmentCleanupOptions> options,
    ILogger<LicenseAssignmentCleanupBackgroundService> logger)
    : BackgroundService
{
    private readonly LicenseAssignmentCleanupOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "License Assignment Cleanup Background Service started. Cleanup frequency: {Frequency} days",
            _options.CleanupFrequencyInDays);

        // Initial delay before first execution (prevents startup load)
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("License Assignment Cleanup Background Service is stopping gracefully.");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in License Assignment Cleanup Background Service. Will retry on next scheduled run.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromDays(_options.CleanupFrequencyInDays), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("License Assignment Cleanup Background Service stopped during delay.");
                break;
            }
        }

        logger.LogInformation("License Assignment Cleanup Background Service stopped.");
    }

    private async Task PerformCleanupAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating service scope for license assignment cleanup.");
        
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<ILicenseAssignmentCleanupService>();

        await cleanupService.CleanupNotUsedAssignmentsAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("License Assignment Cleanup Background Service is stopping...");
        await base.StopAsync(cancellationToken);
    }
}