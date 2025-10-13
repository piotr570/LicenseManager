using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Domain.Licenses.Events;

public class LicenseInvokedEvent(License license, User user) : DomainEvent
{
    public License License { get; init; } = license;
    public User User { get; init; } = user;
}