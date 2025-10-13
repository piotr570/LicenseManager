using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseCanNotAssignExpiredLicenseRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.ExpirationDate <= SystemClock.Now;

    public string? Message => "Cannot assign user to license: license expired.";
}
