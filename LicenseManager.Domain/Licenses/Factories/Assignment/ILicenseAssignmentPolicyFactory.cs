using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Policies;

namespace LicenseManager.Domain.Licenses.Factories.Assignment;

public interface ILicenseAssignmentPolicyFactory
{
    ILicenseAssignmentPolicy GetPolicy(LicenseType type);
}