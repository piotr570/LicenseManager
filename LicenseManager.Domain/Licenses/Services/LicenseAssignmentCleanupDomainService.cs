using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Abstractions;

namespace LicenseManager.Domain.Licenses.Services;

public class LicenseAssignmentCleanupDomainService : ILicenseAssignmentCleanupDomainService
{
    public void CleanupNotUsedAssignments(License license, DateTime currentTime)
    {
        license.CleanupNotUsedAssignments(currentTime);
    }
}