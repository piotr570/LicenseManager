using LicenseManager.Domain.Licenses.Abstractions;

namespace LicenseManager.Domain.Licenses.Services;

public class LicenseAssignmentCleanupDomainService : ILicenseAssignmentCleanupDomainService
{
    public void CleanupNotUsedAssignments(Guid licenseId, DateTime currentTime)
    {
        license.CleanupNotUsedAssignments(currentTime);
    }
}