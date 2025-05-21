namespace LicenseManager.Domain.Licenses.Abstractions;

public interface ILicenseAssignmentCleanupDomainService
{
    void CleanupNotUsedAssignments(License license, DateTime currentTime);
}