using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Services;

public static class RulesEngine
{
    public static IEnumerable<IBusinessRule> GetReservationRules(License license, User user)
    {
        return new List<IBusinessRule>
        {
            new LicenseMustBeActiveRule(license),
            new UserMustNotAlreadyHaveLicenseRule(license, user),
            new LicenseReservationLimitNotExceededRule(license)
        };
    }

    public static IEnumerable<IBusinessRule> GetInvocationRules(License license, User user)
    {
        return new List<IBusinessRule>
        {
            new LicenseMustBeActiveRule(license),
            new LicenseCanNotBeCancelledRule(license),
            new UsageLimitMustNotBeExceededRule(license),
            new LicenseCanNotBeExpiredRule(license),
            new UserMustBeAssignedToLicenseRule(license, user)
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