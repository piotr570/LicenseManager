using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class SingleLicenseCannotHaveMultipleUsersRule(LicenseType licenseType, int? maxUsers) : IBusinessRule
{
    public bool IsBroken() => licenseType == LicenseType.Single && maxUsers != null && maxUsers != 1;

    public string? Message => "A Single license can only have one user.";
}