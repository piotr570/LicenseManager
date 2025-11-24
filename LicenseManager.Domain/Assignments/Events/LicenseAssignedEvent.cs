using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Domain.Assignments.Events;

public class LicenseAssignedEvent(Guid licenseId, Guid userId) : DomainEvent
{
    public Guid LicenseId { get; } = licenseId;
    public Guid UserId { get; } = userId;
}