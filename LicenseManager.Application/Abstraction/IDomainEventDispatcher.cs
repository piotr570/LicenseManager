using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common.Events;

namespace LicenseManager.Application.Abstraction;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent);
}
