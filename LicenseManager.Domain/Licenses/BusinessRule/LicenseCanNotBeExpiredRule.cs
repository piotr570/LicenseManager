using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseCanNotBeExpiredRule(License license) : IBusinessRule
{
    public bool IsBroken() => 
        license.Terms.ExpirationDate.HasValue && license.Terms.ExpirationDate.Value < SystemClock.Now;

    public string? Message => "License has expired and cannot be invoked.";
}