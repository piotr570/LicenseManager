using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class ServerLicenseMustHaveAtLeastOneUserRule(LicenseType licenseType, int? maxUsers) : IBusinessRule
{
    public bool IsBroken() => licenseType == LicenseType.Server && maxUsers is < 1;

    public string? Message => "A Server license must allow at least one user.";
}