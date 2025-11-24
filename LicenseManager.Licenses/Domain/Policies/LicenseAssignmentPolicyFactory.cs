using LicenseManager.Licenses.Domain.Enums;

namespace LicenseManager.Licenses.Domain.Policies;

public interface ILicenseAssignmentPolicyFactory
{
    ILicenseAssignmentPolicy GetPolicy(LicenseType type);
}

public sealed class LicenseAssignmentPolicyFactory : ILicenseAssignmentPolicyFactory
{
    private readonly SingleLicenseAssignmentPolicy _singlePolicy = new();
    private readonly TeamLicenseAssignmentPolicy _teamPolicy = new();
    private readonly ServerLicenseAssignmentPolicy _serverPolicy = new();

    public ILicenseAssignmentPolicy GetPolicy(LicenseType type)
    {
        return type switch
        {
            LicenseType.Single => _singlePolicy,
            LicenseType.Team => _teamPolicy,
            LicenseType.Server => _serverPolicy,
            _ => throw new ArgumentException($"Unknown license type: {type}")
        };
    }
}