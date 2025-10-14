using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Domain.Reservations.Events;

public class LicenseReservedEvent(Guid licenseId, Guid userId, DateTime reservedAt) : DomainEvent
{
    public Guid LicenseId { get; } = licenseId;
    public Guid UserId { get; } = userId;
    public DateTime ReservedAt { get; } = reservedAt;
}