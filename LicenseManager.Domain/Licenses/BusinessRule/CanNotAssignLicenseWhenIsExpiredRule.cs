using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class CanNotAssignLicenseWhenIsExpiredRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.ExpirationDate < SystemClock.Now;

    public string? Message => "Cannot assign user to license: license expired or user already assigned.";
}