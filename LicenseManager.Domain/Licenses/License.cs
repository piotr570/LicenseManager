using LicenseManager.Domain.Assignments;
using LicenseManager.Domain.Assignments.Events;
using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Policies;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Events;
using LicenseManager.Domain.Services;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using LicenseManager.SharedKernel.Exceptions;

namespace LicenseManager.Domain.Licenses;

public sealed class License : Entity
{
    private List<Assignment> _assignments = [];
    private List<LicenseReservation> _reservations = [];

    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();
    public IReadOnlyCollection<LicenseReservation> Reservations => _reservations.AsReadOnly();

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

    public void AssignUser(User user, ILicenseAssignmentPolicy policy, IEnumerable<IBusinessRule> rules, DateTime assignedDate)
    {
        ValidateAssignment(user, rules);
        policy.CanAssignUser(this, user);

        if (_reservations.Any(r => r.UserId == user.Id))
        {
            _reservations = _reservations.Where(r => r.UserId != user.Id).ToList();
        }
        
        _assignments.Add(new Assignment(this, user, assignedDate));
        AddDomainEvent(new LicenseAssignedEvent(this, user));
    }

    public void Invoke(User user)
    {
        ValidateInvocation(user);
        
        var assignment = _assignments.Single(x => x.UserId == user.Id);
        assignment.UpdateLastActivity();
        UsageCount++;
        
        AddDomainEvent(new LicenseInvokedEvent(this, user));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reserve(User user)
    {
        var rules = RulesEngine.GetReservationRules(this, user);
        RulesEngine.EvaluateRules(rules);

        _reservations = _reservations.Append(new LicenseReservation(this, user)).ToList();
        
        AddDomainEvent(new LicenseReservedEvent(this, user));
    }

    public int CleanupExpiredReservations()
    {
        var expiredReservations = _reservations.Where(r => r.IsExpired(SystemClock.Now)).ToList();
        _reservations = _reservations.Except(expiredReservations).ToList();
        return expiredReservations.Count;
    }

    public void CleanupNotUsedAssignments(DateTime currentTime)
    {
        var notUsedAssignments = Assignments.Where(a => a.State == AssignmentState.NotUsed).ToList();

        foreach (var assignment in notUsedAssignments)
        {
            _assignments.Remove(assignment);
            AddDomainEvent(new AssignmentRemovedEvent(assignment.Id, Id));
        }
    }

    private IEnumerable<IBusinessRule> GetInvocationRules(User user)
        => RulesEngine.GetInvocationRules(this, user);

    private static void ValidateLicenseCreation(string key, string vendor, string name, LicenseTerms terms)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("License key is required.", nameof(key));

        if (string.IsNullOrWhiteSpace(vendor))
            throw new ArgumentException("Vendor name is required.", nameof(vendor));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("License name is required.", nameof(name));

        if (terms == null)
            throw new ArgumentNullException(nameof(terms), "License terms must be provided.");
    }

    private void ValidateAssignment(User user, IEnumerable<IBusinessRule> rules)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User is required for license assignment.");

        foreach (var rule in rules)
        {
            if (rule.IsBroken())
                throw new BusinessRuleViolationException(rule);
        }
    }

    private void ValidateInvocation(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User is required to invoke the license.");
        }

        if (!IsActive)
        {
            throw new LicenseNotActiveException("Cannot invoke an inactive license.");
        }
        
        var rules = GetInvocationRules(user);
        foreach (var rule in rules)
        {
            if (rule.IsBroken())
                throw new BusinessRuleViolationException(rule);
        }
    }
}