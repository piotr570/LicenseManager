using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class UsageLimitMustNotBeExceededRule(License license) : IBusinessRule
{
    private readonly License _license = license ?? throw new ArgumentNullException(nameof(license));

    public bool IsBroken() => 
        _license.Terms is { Mode: LicenseMode.UsageBased, UsageLimit: not null } 
        && _license.UsageCount >= _license.Terms.UsageLimit.Value;

    public string? Message => "License usage limit has been exceeded.";
}