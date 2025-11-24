using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Licenses.Domain.Events;

public sealed record LicenseReservedEvent(Guid LicenseId, Guid UserId) : IDomainEvent
{
    public Guid AggregateId => LicenseId;
    public DateTime OccurredOn => DateTime.UtcNow;
}