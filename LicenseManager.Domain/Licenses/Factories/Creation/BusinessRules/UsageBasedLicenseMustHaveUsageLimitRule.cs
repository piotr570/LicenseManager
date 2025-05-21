using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class UsageBasedLicenseMustHaveUsageLimitRule(LicenseMode licenseMode, int? usageLimit) : IBusinessRule
{
    public bool IsBroken() => licenseMode == LicenseMode.UsageBased && !usageLimit.HasValue;

    public string? Message => "A Usage-based license must have a usage limit.";
}