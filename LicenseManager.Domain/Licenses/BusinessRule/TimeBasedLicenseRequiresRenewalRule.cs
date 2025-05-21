using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class TimeBasedLicenseRequiresRenewalRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms is { IsRenewable: true, RenewalDate: null };

    public string? Message => "Time-based licenses that are renewable must have a renewal date.";
}