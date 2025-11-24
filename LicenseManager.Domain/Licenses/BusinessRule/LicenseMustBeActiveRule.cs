using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Rules;

public class LicenseMustBeActiveRule(bool isActive) : IBusinessRule
{
    public bool IsBroken() => !isActive;
    public string Message => "License must be active to be invoked.";
}