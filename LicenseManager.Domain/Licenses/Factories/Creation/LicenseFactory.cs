using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common.BusinessRules;
using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

namespace LicenseManager.Domain.Licenses.Factories.Creation;

internal class LicenseFactory : ILicenseFactory
{
    public License Create(string key, string vendor, string name, LicenseType licenseType, 
        LicenseMode licenseMode, int? maxUsers, bool isRenewable, DateTime? expirationDate, 
        DateTime? renewalDate, int? usageLimit)
    {
        CheckBusinessRules([
            new LicenseKeyCannotBeEmptyRule(key),
            new ValueCannotBeEmptyRule(nameof(name), name),
            new ValueCannotBeEmptyRule(nameof(vendor), vendor),
            new SingleLicenseCannotHaveMultipleUsersRule(licenseType, maxUsers),
            new TeamLicenseMustHaveMoreThanOneUserRule(licenseType, maxUsers),
            new ServerLicenseMustHaveAtLeastOneUserRule(licenseType, maxUsers),
            new NoExpirationForUsageBasedLicenseRule(licenseMode, expirationDate),
            new UsageBasedLicenseMustHaveUsageLimitRule(licenseMode, usageLimit),
            new TimeBasedLicenseMustHaveExpirationDateRule(licenseMode, expirationDate),
            new SubscriptionLicenseMustBeRenewableRule(licenseMode, isRenewable),
            new RenewalDateMustBeAfterExpirationDateRule(expirationDate, renewalDate)
        ]);

        var licenseId = Guid.NewGuid();
        var terms = new LicenseTerms(licenseId, licenseType, licenseMode, maxUsers, isRenewable, expirationDate, renewalDate, usageLimit);
        var license = License.Create(licenseId, key, vendor, name, terms);
        
        return license;
    }
    
    private static void CheckBusinessRules(IEnumerable<IBusinessRule> rules)
    {
        var brokenRules = rules.Where(rule => rule.IsBroken()).ToList();

        if (brokenRules.Any())
        {
            throw new BusinessRuleViolationException(brokenRules.First());
        }
    }
}