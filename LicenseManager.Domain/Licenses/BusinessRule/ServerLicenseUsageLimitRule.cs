using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class ServerLicenseUsageLimitRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.Type == LicenseType.Server && license.UsageCount > license.Terms.UsageLimit;

    public string? Message => "Server license usage limit has been exceeded.";
}