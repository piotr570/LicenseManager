using System.Collections.ObjectModel;
using LicenseManager.SharedKernel.Common;
using LicenseManager.SharedKernel.Exceptions;
using LicenseManager.SharedKernel.Events;

namespace LicenseManager.SharedKernel.Abstractions;

public abstract class Entity : IEquatable<Entity>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public ReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; } = SystemClock.Now;
    public bool HasChanges => DomainEvents.Count != 0;

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    protected void CheckBusinessRules(IEnumerable<IBusinessRule> rules)
    {
        var brokenRules = rules.Where(rule => rule.IsBroken()).ToList();

        if (brokenRules.Any())
        {
            throw new BusinessRuleViolationException(brokenRules.First());
        }
    }

    public bool Equals(Entity? other)
    {
        return other != null && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Id.Equals(entity.Id);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}