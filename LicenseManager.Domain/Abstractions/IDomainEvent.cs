namespace LicenseManager.Domain.Abstractions;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}