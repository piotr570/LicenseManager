using LicenseManager.Application.Abstraction;
using LicenseManager.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Infrastructure.Persistence;

public class UnitOfWork<TContext>(TContext dbContext, 
    IDomainEventDispatcher domainEventDispatcher)
    : IUnitOfWork
    where TContext : DbContext
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var entitiesWithEvents = dbContext.ChangeTracker
            .Entries<Entity>() 
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var result = await dbContext.SaveChangesAsync(cancellationToken);

        await DispatchEvents(entitiesWithEvents);

        return result;
    }

    private async Task DispatchEvents(IEnumerable<Entity> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            foreach (var domainEvent in entity.DomainEvents.ToList())
            {
                await domainEventDispatcher.Dispatch(domainEvent);
            }

            entity.ClearDomainEvents(); 
        }
    }
}