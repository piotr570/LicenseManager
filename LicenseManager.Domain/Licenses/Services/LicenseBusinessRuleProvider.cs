using LicenseManager.Domain.Licenses.Abstractions;
using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Services;

public class LicenseBusinessRuleProvider : ILicenseBusinessRuleProvider
{
    private readonly Dictionary<LicenseMode, Func<License, User?, IEnumerable<IBusinessRule>>> _licenseModeRules =
        new()
        {
            {
                LicenseMode.TimeBased, (license, _) => new List<IBusinessRule>
                {
                    new LicenseCanNotAssignExpiredLicenseRule(license),
                    new TimeBasedLicenseRequiresRenewalRule(license) 
                }
            },
            {
                LicenseMode.UsageBased, (license, _) => new List<IBusinessRule>
                {
                    new UsageBasedLicenseCanNotHaveExpirationRule(license), 
                    new LicenseUsageLimitCanNotBeExceededRule(license) 
                }
            },
            {
                LicenseMode.SubscriptionBased, (license, _) => new List<IBusinessRule>
                {
                    new SubscriptionPaymentValidRule(license)
                }
            }
        };

    private readonly Dictionary<LicenseType, Func<License, User, IEnumerable<IBusinessRule>>> _licenseTypeRules =
        new()
        {
            {
                LicenseType.Single, (license, user) => new List<IBusinessRule>
                {
                    new SingleTypeLicenseCannotBeAssignedWhenAlreadyAssigned(license, user),
                    new CanNotAssignLicenseWhenIsExpiredRule(license),
                    new CanNotAssignSingleTypeLicenseWhenReserved(license),
                    new LicenseMustBeAssignedToOneUserOnlyRule(license) // Ensures single-user licenses are assigned only once
                }
            },
            {
                LicenseType.Team, (license, user) => new List<IBusinessRule>
                {
                    new CanNotAssignLicenseWhenIsExpiredRule(license),
                    new TeamLicenseMaxUsersRule(license) 
                }
            },
            {
                LicenseType.Server, (license, _) => new List<IBusinessRule>
                {
                    new ServerLicenseUsageLimitRule(license), 
                    new CanNotAssignLicenseWhenIsExpiredRule(license)
                }
            }
        };

    public IEnumerable<IBusinessRule> GetRules(License license, User user)
    {
        var rules = new List<IBusinessRule>();

        if (_licenseModeRules.TryGetValue(license.Terms.Mode, out var modeRuleFactory))
        {
            rules.AddRange(modeRuleFactory(license, user));
        }

        if (_licenseTypeRules.TryGetValue(license.Terms.Type, out var typeRuleFactory))
        {
            rules.AddRange(typeRuleFactory(license, user));
        }

        return rules;
    }

    public IEnumerable<IBusinessRule> GetDeletionRules(License license)
    {
        return
        [
            new CannotDeleteAssignedLicenseRule(license)
        ];
    }
}