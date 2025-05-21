using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;

public class RenewalDateMustBeAfterExpirationDateRule(DateTime? expirationDate, DateTime? renewalDate) : IBusinessRule
{
    public bool IsBroken() =>
        expirationDate.HasValue &&
        renewalDate.HasValue &&
        renewalDate <= expirationDate;

    public string? Message => "Renewal date must be after the expiration date.";
}