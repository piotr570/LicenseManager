using LicenseManager.Domain.Common.Events;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Events;

public class LicenseReservedEvent(License license, User user) : DomainEvent
{
    public Guid LicenseId { get; } = license.Id;
    public Guid UserId { get; } = user.Id;
}