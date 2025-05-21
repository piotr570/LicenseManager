using LicenseManager.Application.HostedServices.Interfaces;
using LicenseManager.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LicenseManager.Infrastructure.BackgroundServices;

public class LicenseAssignmentCleanupBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<LicenseAssignmentCleanupSettings> options,
    ILogger<LicenseAssignmentCleanupBackgroundService> logger)
    : BackgroundService
{
    private readonly LicenseAssignmentCleanupSettings _settings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("License Assignment Cleanup Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during License Assignment Cleanup.");
            }

            await Task.Delay(TimeSpan.FromDays(_settings.CleanupFrequencyInDays), stoppingToken);
        }

        logger.LogInformation("License Assignment Cleanup Background Service stopped.");
    }

    private async Task PerformCleanupAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var cleanupService = scope.ServiceProvider.GetRequiredService<ILicenseAssignmentCleanupService>();

        await cleanupService.CleanupNotUsedAssignmentsAsync(cancellationToken);
    }
}