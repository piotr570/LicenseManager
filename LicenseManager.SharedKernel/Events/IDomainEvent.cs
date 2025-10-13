namespace LicenseManager.SharedKernel.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}