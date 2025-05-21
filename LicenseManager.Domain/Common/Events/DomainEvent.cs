using LicenseManager.Domain.Abstractions;

namespace LicenseManager.Domain.Common.Events;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = SystemClock.Now;
}