using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class TeamLicenseMustHaveMoreThanOneUserRule(LicenseType licenseType, int? maxUsers) : IBusinessRule
{
    public bool IsBroken() => licenseType == LicenseType.Team && maxUsers is < 2;

    public string? Message => "A Team license must allow at least two users.";
}