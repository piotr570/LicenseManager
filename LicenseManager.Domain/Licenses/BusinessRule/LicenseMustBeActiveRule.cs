using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseMustBeActiveRule(License license) : IBusinessRule
{
    public bool IsBroken() => !license.IsActive;

    public string? Message => "License must be active to be invoked.";
}