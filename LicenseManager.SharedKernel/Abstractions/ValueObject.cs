using LicenseManager.SharedKernel.Exceptions;

namespace LicenseManager.SharedKernel.Abstractions;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other)
    {
        if (other is null) return false;
        if (GetType() != other.GetType()) return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && Equals(other);
    }

    public override int GetHashCode()
    {
        // Combine hash codes of components in a stable way
        unchecked
        {
            const int seed = 17;
            const int multiplier = 31;
            return GetEqualityComponents().Aggregate(seed, (current, obj) => (current * multiplier) + (obj?.GetHashCode() ?? 0));
        }
    }

    protected void CheckBusinessRules(IEnumerable<IBusinessRule> rules)
    {
        var brokenRules = rules.Where(x => x.IsBroken()).ToList();

        if (brokenRules.Any())
        {
            throw new BusinessRuleViolationException(brokenRules.First());
        }
    }
}
