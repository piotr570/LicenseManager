using LicenseManager.Application.HostedServices.Interfaces;
using LicenseManager.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LicenseManager.Infrastructure.BackgroundServices;

public class LicenseReservationCleanupBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<LicenseReservationCleanupSettings> options,
    ILogger<LicenseReservationCleanupBackgroundService> logger)
    : BackgroundService
{
    private readonly LicenseReservationCleanupSettings _settings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("License Reservation Cleanup Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during License Reservation Cleanup.");
            }

            // Delay for configured interval (1 hour)
            await Task.Delay(TimeSpan.FromHours(_settings.CleanupFrequencyInDays), stoppingToken);
        }

        logger.LogInformation("License Reservation Cleanup Background Service stopped.");
    }

    private async Task PerformCleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<ILicenseReservationCleanupService>();
        
        await cleanupService.CleanupExpiredReservationsAsync(cancellationToken);
    }
}