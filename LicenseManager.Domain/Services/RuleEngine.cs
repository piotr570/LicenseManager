using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;

namespace LicenseManager.Domain.Services;

public static class RulesEngine
{
    public static IEnumerable<IBusinessRule> GetReservationRules(Guid licenseId, Guid userId)
    {
        return new List<IBusinessRule>
        {
            new LicenseMustBeActiveRule(licenseId),
            new UserMustNotAlreadyHaveLicenseRule(licenseId, userId),
            new LicenseReservationLimitNotExceededRule(licenseId)
        };
    }

    public static IEnumerable<IBusinessRule> GetInvocationRules(Guid licenseId, Guid userId)
    {
        return new List<IBusinessRule>
        {
            new LicenseMustBeActiveRule(licenseId),
            new LicenseCanNotBeCancelledRule(licenseId),
            new UsageLimitMustNotBeExceededRule(licenseId),
            new LicenseCanNotBeExpiredRule(licenseId),
            new UserMustBeAssignedToLicenseRule(licenseId, userId)
        };
    }

    public static void EvaluateRules(IEnumerable<IBusinessRule> rules)
    {
        foreach (var rule in rules)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleViolationException(rule);
            }
        }
    }
}