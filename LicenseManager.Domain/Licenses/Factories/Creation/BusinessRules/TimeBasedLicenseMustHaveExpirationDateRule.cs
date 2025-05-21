using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class TimeBasedLicenseMustHaveExpirationDateRule(LicenseMode licenseMode, DateTime? expirationDate) : IBusinessRule
{
    public bool IsBroken() => licenseMode == LicenseMode.TimeBased && !expirationDate.HasValue;

    public string? Message => "A Time-based license must have an expiration date.";
}