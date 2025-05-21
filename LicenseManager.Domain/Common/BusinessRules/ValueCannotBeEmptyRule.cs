using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Common.BusinessRules;

public class ValueCannotBeEmptyRule(string fieldName, string value) : IBusinessRule
{
    public bool IsBroken() => string.IsNullOrWhiteSpace(value);

    public string? Message => string.Format("{0} cannot be empty.", fieldName);
}