namespace LicenseManager.Domain.Licenses.Abstractions;

public interface ILicenseAssignmentCleanupDomainService
{
    void CleanupNotUsedAssignments(Guid licenseId, DateTime currentTime);
}