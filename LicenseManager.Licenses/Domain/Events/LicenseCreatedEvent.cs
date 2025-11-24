using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Licenses.Domain.Events;

public sealed record LicenseCreatedEvent(Guid LicenseId, string Key, string Name) : IDomainEvent
{
    public Guid AggregateId => LicenseId;
    public DateTime OccurredOn => DateTime.UtcNow;
}