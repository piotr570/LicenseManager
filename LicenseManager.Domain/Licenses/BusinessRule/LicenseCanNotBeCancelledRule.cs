using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseCanNotBeCancelledRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.IsCancelled;

    public string? Message => "Cancelled licenses cannot be invoked.";
}