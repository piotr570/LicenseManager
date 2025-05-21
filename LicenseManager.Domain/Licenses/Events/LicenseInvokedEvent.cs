using LicenseManager.Domain.Common;
using LicenseManager.Domain.Common.Events;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Events;

public class LicenseInvokedEvent(License license, User user) : DomainEvent
{
    public License License { get; init; } = license;
    public User User { get; init; } = user;
}