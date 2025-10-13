using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Domain.Licenses.Events;

public class LicenseReservedEvent(License license, User user) : DomainEvent
{
    public Guid LicenseId { get; } = license.Id;
    public Guid UserId { get; } = user.Id;
}