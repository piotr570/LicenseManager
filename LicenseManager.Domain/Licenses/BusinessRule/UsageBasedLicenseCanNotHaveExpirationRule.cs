using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class UsageBasedLicenseCanNotHaveExpirationRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms is { Mode: LicenseMode.UsageBased, ExpirationDate: not null };

    public string? Message => "Usage-based licenses should not have an expiration date.";
}