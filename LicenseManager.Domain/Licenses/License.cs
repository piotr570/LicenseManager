using LicenseManager.Domain.Assignments.Events;
using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Policies;
using LicenseManager.Domain.Licenses.Events;
using LicenseManager.Domain.Services;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;

namespace LicenseManager.Domain.Licenses;

public sealed class License : Entity, IAggregateRoot
{
    private readonly List<Guid> _assignmentIds = [];
    private readonly List<Guid> _reservationIds = [];
    public IReadOnlyCollection<Guid> AssignmentIds => _assignmentIds.AsReadOnly();
    public IReadOnlyCollection<Guid> ReservationIds => _reservationIds.AsReadOnly();

    public string Key { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Vendor { get; private set; } = null!;
    public LicenseTerms Terms { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int UsageCount { get; private set; }
    public bool? IsPaymentValid { get; private set; }
    public bool IsCancelled { get; private set; }
    public DepartmentType? Department { get; private set; }

    // Constructor (Private for Domain Integrity)
    private License() { }

    private License(Guid id, string key, string vendor, string name, LicenseTerms terms)
    {
        ValidateLicenseCreation(key, vendor, name, terms);

        Id = id; 
        Key = key;
        Vendor = vendor;
        Name = name;
        Terms = terms;
        IsActive = true;
    }

    public static License Create(Guid id, string key, string vendor, string name, LicenseTerms terms)
    {
        return new License(id, key, vendor, name, terms);
    }

    public void AssignUser(Guid userId, ILicenseAssignmentPolicy policy, IEnumerable<IBusinessRule> rules, DateTime assignedDate)
    {
        ValidateAssignment(userId, rules);
        policy.CanAssignUser(Id, userId);

        // Remove reservation if exists
        _reservationIds.Remove(userId);
        // Add assignment
        _assignmentIds.Add(userId);
        AddDomainEvent(new LicenseAssignedEvent(Id, userId));
    }

    public void Invoke(Guid userId)
    {
        ValidateInvocation(userId);
        UsageCount++;
        AddDomainEvent(new LicenseInvokedEvent(Id, userId));
    }

    public void Deactivate() => IsActive = false;

    public void Reserve(Guid userId)
    {
        var rules = RulesEngine.GetReservationRules(this, userId);
        RulesEngine.EvaluateRules(rules);
        _reservationIds.Add(userId);
        AddDomainEvent(new LicenseReservedEvent(Id, userId));
    }

    public int CleanupExpiredReservations(Func<Guid, bool> isExpired)
    {
        var expired = _reservationIds.Where(isExpired).ToList();
        foreach (var userId in expired)
            _reservationIds.Remove(userId);
        return expired.Count;
    }

    public void CleanupNotUsedAssignments(Func<Guid, bool> isNotUsed)
    {
        var notUsed = _assignmentIds.Where(isNotUsed).ToList();
        foreach (var userId in notUsed)
        {
            _assignmentIds.Remove(userId);
            AddDomainEvent(new AssignmentRemovedEvent(userId, Id));
        }
    }

    private IEnumerable<IBusinessRule> GetInvocationRules(Guid userId)
        => RulesEngine.GetInvocationRules(this, userId);

    private static void ValidateLicenseCreation(string key, string vendor, string name, LicenseTerms terms)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("License key is required.", nameof(key));
        if (string.IsNullOrWhiteSpace(vendor)) throw new ArgumentException("Vendor name is required.", nameof(vendor));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("License name is required.", nameof(name));
        if (terms == null) throw new ArgumentNullException(nameof(terms), "License terms must be provided.");
    }

    private void ValidateAssignment(Guid userId, IEnumerable<IBusinessRule> rules)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId), "UserId is required for license assignment.");
        foreach (var rule in rules)
            if (rule.IsBroken()) throw new BusinessRuleViolationException(rule);
    }

    private void ValidateInvocation(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId), "UserId is required to invoke the license.");
        if (!IsActive) throw new LicenseNotActiveException("Cannot invoke an inactive license.");
        var rules = GetInvocationRules(userId);
        foreach (var rule in rules)
            if (rule.IsBroken()) throw new BusinessRuleViolationException(rule);
    }
}