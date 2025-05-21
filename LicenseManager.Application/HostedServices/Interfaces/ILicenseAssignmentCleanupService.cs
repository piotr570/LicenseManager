namespace LicenseManager.Application.HostedServices.Interfaces;

public interface ILicenseAssignmentCleanupService
{
    Task CleanupNotUsedAssignmentsAsync(CancellationToken cancellationToken);
}