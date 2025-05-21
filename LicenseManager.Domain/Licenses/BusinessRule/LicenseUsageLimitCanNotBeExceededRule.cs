using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseUsageLimitCanNotBeExceededRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.UsageLimit.HasValue && license.UsageCount >= license.Terms.UsageLimit.Value;

    public string? Message => "Usage-based license has exceeded its usage limit.";
}