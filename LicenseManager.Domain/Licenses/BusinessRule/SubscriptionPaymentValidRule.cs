using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class SubscriptionPaymentValidRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.Mode == LicenseMode.SubscriptionBased 
                              && license.IsPaymentValid.HasValue && license.IsPaymentValid.Value;

    public string? Message => "Subscription-based licenses must have a valid payment.";
}