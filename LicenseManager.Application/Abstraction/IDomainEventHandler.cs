using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common.Events;

namespace LicenseManager.Application.Abstraction;

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent);
}