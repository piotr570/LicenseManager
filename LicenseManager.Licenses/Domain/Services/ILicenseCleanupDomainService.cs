namespace LicenseManager.Licenses.Domain.Services;

internal interface ILicenseAssignmentCleanupDomainService
{
    Task<int> CleanupUnusedAssignmentsAsync(int daysInactive, CancellationToken cancellationToken = default);
}