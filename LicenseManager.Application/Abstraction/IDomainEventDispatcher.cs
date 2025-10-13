using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Application.Abstraction;

public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent domainEvent);
}
