using LicenseManager.Domain.Common.Events;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Assignments.Events;

public class LicenseAssignedEvent(License license, User user) : DomainEvent
{
    public License License { get; } = license;
    public User User { get; } = user;
}