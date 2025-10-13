using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.SharedKernel.Exceptions;

public class BusinessRuleViolationException(IBusinessRule brokenRule) : Exception(brokenRule.Message)
{
    public IBusinessRule BrokenRule { get; } = brokenRule;
}