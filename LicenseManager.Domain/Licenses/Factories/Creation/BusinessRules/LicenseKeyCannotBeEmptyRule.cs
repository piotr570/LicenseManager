using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class LicenseKeyCannotBeEmptyRule(string key) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(key);

    public string? Message => "License key cannot be empty.";
}