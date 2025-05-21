using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Common.Exceptions;

public class BusinessRuleViolationException(IBusinessRule brokenRule) : Exception(brokenRule.Message)
{
    public IBusinessRule BrokenRule { get; } = brokenRule;
}