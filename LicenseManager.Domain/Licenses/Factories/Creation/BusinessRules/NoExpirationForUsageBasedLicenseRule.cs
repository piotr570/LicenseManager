using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class NoExpirationForUsageBasedLicenseRule(LicenseMode licenseMode, DateTime? expirationDate) : IBusinessRule
{
    public bool IsBroken() => licenseMode == LicenseMode.UsageBased &&
                              expirationDate.HasValue;

    public string? Message => "Usage-based licenses should not have an expiration date.";
}