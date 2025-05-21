using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Policies;

namespace LicenseManager.Domain.Licenses.Factories.Assignment;

public class LicenseAssignmentPolicyFactory : ILicenseAssignmentPolicyFactory
{
    private readonly Dictionary<LicenseType, ILicenseAssignmentPolicy> _policies = new()
    {
        { LicenseType.Single, new SingleLicenseAssignmentPolicy() },
        { LicenseType.Team, new TeamLicenseAssignmentPolicy() },
        { LicenseType.Server, new ServerLicenseAssignmentPolicy() }
    };

    public ILicenseAssignmentPolicy GetPolicy(LicenseType type)
    {
        if (!_policies.TryGetValue(type, out var policy))
        {
            throw new ArgumentException($"No policy found for license type: {type}");
        }
        
        return policy;
    }
}