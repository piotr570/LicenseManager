using LicenseManager.SharedKernel.Common;

namespace LicenseManager.SharedKernel.Events;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = SystemClock.Now;
}