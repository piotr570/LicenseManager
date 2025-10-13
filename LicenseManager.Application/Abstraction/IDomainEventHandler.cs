using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Application.Abstraction;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent);
}