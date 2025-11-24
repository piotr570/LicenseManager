using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Licenses.Domain.Events;

public sealed record LicenseInvokedEvent(Guid LicenseId, Guid UserId) : IDomainEvent
{
    public Guid AggregateId => LicenseId;
    public DateTime OccurredOn => DateTime.UtcNow;
}