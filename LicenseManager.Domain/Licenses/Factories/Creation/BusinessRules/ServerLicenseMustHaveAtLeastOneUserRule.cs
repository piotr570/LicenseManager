using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class ServerLicenseMustHaveAtLeastOneUserRule(LicenseType licenseType, int? maxUsers) : IBusinessRule
{
    public bool IsBroken() => licenseType == LicenseType.Server && maxUsers is < 1;

    public string? Message => "A Server license must allow at least one user.";
}