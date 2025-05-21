using LicenseManager.Application.HostedServices.Interfaces;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Abstractions;
using LicenseManager.Domain.Licenses.Services;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.HostedServices;

public class LicenseAssignmentCleanupService(
    IRepository<License> repository,
    IUnitOfWork unitOfWork,
    ILogger<LicenseAssignmentCleanupService> logger,
    ILicenseAssignmentCleanupDomainService cleanupDomainService)
    : ILicenseAssignmentCleanupService
{
    public async Task CleanupNotUsedAssignmentsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Running Not Used License Assignments Cleanup Service");

        var licenses = await repository
            .GetAllIncludingAsync(x => x.Assignments.Count > 0);

        foreach (var license in licenses)
        {
            cleanupDomainService.CleanupNotUsedAssignments(license, SystemClock.Now);

            if (license.HasChanges)
            {
                logger.LogInformation("Cleaned up unused assignments for {0} license.", license.Id);
                repository.Update(license);
            }
        }

        var hasChanges = licenses.Any(license => license.HasChanges);

        if (hasChanges)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            logger.LogInformation("No changes detected. Skipping database save operation.");
        }
    }
}