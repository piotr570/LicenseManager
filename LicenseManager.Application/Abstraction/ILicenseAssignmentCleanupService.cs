namespace LicenseManager.Application.Abstraction;

public interface ILicenseAssignmentCleanupService
{
    Task CleanupNotUsedAssignmentsAsync(CancellationToken cancellationToken);
}