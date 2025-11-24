using LicenseManager.Domain.Assignments;
using LicenseManager.Domain.Assignments.Events;
using LicenseManager.Domain.Licenses.Events;
using LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.BusinessRules;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseManager.Domain.Licenses;

public sealed class License : Entity, IAggregateRoot
{
    private readonly List<Assignment> _assignments = new();
    private readonly List<Guid> _reservationIds = [];

    // Expose assignments as a read-only navigation property (EF will map via backing field)
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

    [NotMapped]
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

    private License() { }

    public License(string key, string vendor, string name, LicenseTerms terms)
    {
        CheckBusinessRules([
            new LicenseKeyCannotBeEmptyRule(key),
            new ValueCannotBeEmptyRule(nameof(name), name),
            new ValueCannotBeEmptyRule(nameof(vendor), vendor)
        ]);
        
        Key = key;
        Vendor = vendor;
        Name = name;
        Terms = terms;
        IsActive = true;
    }

    public void Reserve(Guid userId)
    {
        _reservationIds.Add(userId);
        AddDomainEvent(new LicenseReservedEvent(Id, userId));
    }

    public void Invoke(Guid userId)
    {
        UsageCount++;
        AddDomainEvent(new LicenseInvokedEvent(Id, userId));
    }

    public void AssignUser(Assignment assignment)
    {
        // Remove reservation if exists
        // _reservationIds.Remove(assignment.);
        _assignments.Add(assignment);
        AddDomainEvent(new LicenseAssignedEvent(Id, assignment.UserId));
    }

    public void Deactivate() => IsActive = false;

    public int CleanupExpiredReservations(Func<Guid, bool> isExpired)
    {
        var expired = _reservationIds.Where(isExpired).ToList();
        foreach (var userId in expired)
            _reservationIds.Remove(userId);
        return expired.Count;
    }

    public int CleanupNotUsedAssignments(Func<Assignment, bool> isNotUsed)
    {
        var notUsed = _assignments.Where(isNotUsed).ToList();
        foreach (var assignment in notUsed)
        {
            _assignments.Remove(assignment);
            AddDomainEvent(new AssignmentRemovedEvent(assignment.Id, Id));
        }
        return notUsed.Count;
    }

    private static void ValidateLicenseCreation(string key, string vendor, string name, LicenseTerms terms)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("License key is required.", nameof(key));
        if (string.IsNullOrWhiteSpace(vendor)) throw new ArgumentException("Vendor name is required.", nameof(vendor));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("License name is required.", nameof(name));
        if (terms == null) throw new ArgumentNullException(nameof(terms), "License terms must be provided.");
    }
}