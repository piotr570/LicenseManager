using LicenseManager.Application.Abstraction;
using LicenseManager.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Infrastructure.Services;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task Dispatch(IDomainEvent domainEvent)
    {
        var domainEventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEventType);

        var handlers = serviceProvider.GetServices(handlerType);

        var dispatchTasks = handlers.Select(handler =>
            (Task)handlerType.GetMethod("Handle")!.Invoke(handler, [domainEvent])!
        );

        await Task.WhenAll(dispatchTasks);
    }
}
