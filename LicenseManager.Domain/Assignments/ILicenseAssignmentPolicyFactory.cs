using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Rules;

namespace LicenseManager.Domain.Assignments;

public interface ILicenseAssignmentPolicyFactory
{
    ILicenseAssignmentPolicy GetPolicy(LicenseType type);
}