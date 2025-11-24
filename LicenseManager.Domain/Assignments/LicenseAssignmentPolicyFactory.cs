using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Policies;
using LicenseManager.Domain.Licenses.Rules;

namespace LicenseManager.Domain.Assignments;

public class LicenseAssignmentPolicyFactory(
    SingleLicenseAssignmentPolicy singlePolicy,
    TeamLicenseAssignmentPolicy teamPolicy,
    ServerLicenseAssignmentPolicy serverPolicy)
    : ILicenseAssignmentPolicyFactory
{
    private readonly IDictionary<LicenseType, ILicenseAssignmentPolicy> _policies = new Dictionary<LicenseType, ILicenseAssignmentPolicy>
    {
        { LicenseType.Single, singlePolicy },
        { LicenseType.Team, teamPolicy },
        { LicenseType.Server, serverPolicy }
    };

    public ILicenseAssignmentPolicy GetPolicy(LicenseType type)
    {
        return _policies.TryGetValue(type, out var policy) 
            ? policy 
            : throw new ArgumentException($"No policy found for license type: {type}");
    }
}