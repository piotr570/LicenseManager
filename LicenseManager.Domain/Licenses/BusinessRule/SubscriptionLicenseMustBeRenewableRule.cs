using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class SubscriptionLicenseMustBeRenewableRule(LicenseMode licenseMode, bool isRenewable) : IBusinessRule
{
    public bool IsBroken() => licenseMode == LicenseMode.SubscriptionBased && !isRenewable;

    public string? Message => "A Subscription-based license must be renewable.";
}