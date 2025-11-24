using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

namespace LicenseManager.Domain.Licenses;

public class LicenseTerms : ValueObject
{
    public int? MaxUsers { get; private set; }
    public int? UsageLimit { get; private set; }
    public LicenseType Type { get; private set; }
    public LicenseMode Mode { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public bool IsRenewable { get; private set; } 
    public DateTime? RenewalDate { get; private set; }
    
    public LicenseTerms(
        LicenseType type, 
        LicenseMode mode, 
        int? maxUsers, 
        bool isRenewable, 
        DateTime? expirationDate, 
        DateTime? renewalDate, 
        int? usageLimit)
    {
        CheckBusinessRules([
            new SingleLicenseCannotHaveMultipleUsersRule(type, maxUsers),
            new TeamLicenseMustHaveMoreThanOneUserRule(type, maxUsers),
            new ServerLicenseMustHaveAtLeastOneUserRule(type, maxUsers),
            new NoExpirationForUsageBasedLicenseRule(mode, expirationDate),
            new UsageBasedLicenseMustHaveUsageLimitRule(mode, usageLimit),
            new TimeBasedLicenseMustHaveExpirationDateRule(mode, expirationDate),
            new SubscriptionLicenseMustBeRenewableRule(mode, isRenewable),
            new RenewalDateMustBeAfterExpirationDateRule(expirationDate, renewalDate)
        ]);
        
        MaxUsers = maxUsers;
        UsageLimit = usageLimit;
        Type = type;
        Mode = mode;
        ExpirationDate = expirationDate;
        IsRenewable = isRenewable;
        RenewalDate = renewalDate;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MaxUsers;
        yield return UsageLimit;
        yield return Type;
        yield return Mode;
        yield return ExpirationDate;
        yield return IsRenewable;
        yield return RenewalDate;
    }
}