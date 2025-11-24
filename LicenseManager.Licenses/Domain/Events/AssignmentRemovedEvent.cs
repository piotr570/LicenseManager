using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Licenses.Domain.Events;

public sealed record AssignmentRemovedEvent(Guid AssignmentId, Guid LicenseId) : IDomainEvent
{
    public Guid AggregateId => LicenseId;
    public DateTime OccurredOn => DateTime.UtcNow;
}

