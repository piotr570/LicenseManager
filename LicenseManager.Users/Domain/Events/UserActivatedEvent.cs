using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Users.Domain.Events;

public sealed record UserActivatedEvent(Guid UserId) : IDomainEvent
{
    public Guid AggregateId => UserId;
    public DateTime OccurredOn => DateTime.UtcNow;
}

