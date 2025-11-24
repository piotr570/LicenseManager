using LicenseManager.Licenses.Domain.Events;
using LicenseManager.Licenses.Domain.ValueObjects;
using LicenseManager.SharedKernel.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseManager.Licenses.Domain.Aggregates;

public sealed class License : Entity, IAggregateRoot
{
    private readonly List<Assignment> _assignments = new();
    private readonly List<Guid> _reservationIds = [];

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
    public Guid? DepartmentId { get; private set; }

    private License() { }

    public License(string key, string vendor, string name, LicenseTerms terms, Guid? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("License key is required.", nameof(key));
        if (string.IsNullOrWhiteSpace(vendor))
            throw new ArgumentException("Vendor name is required.", nameof(vendor));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("License name is required.", nameof(name));
        if (terms == null)
            throw new ArgumentNullException(nameof(terms), "License terms must be provided.");

        Key = key;
        Vendor = vendor;
        Name = name;
        Terms = terms;
        DepartmentId = departmentId;
        IsActive = true;

        AddDomainEvent(new LicenseCreatedEvent(Id, Key, Name));
    }

    public void Reserve(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (!_reservationIds.Contains(userId))
        {
            _reservationIds.Add(userId);
            AddDomainEvent(new LicenseReservedEvent(Id, userId));
        }
    }

    public void Invoke(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        if (!IsActive)
            throw new InvalidOperationException("Cannot invoke inactive license.");

        UsageCount++;
        AddDomainEvent(new LicenseInvokedEvent(Id, userId));
    }

    public void AssignUser(Assignment assignment)
    {
        if (assignment == null)
            throw new ArgumentNullException(nameof(assignment));

        if (assignment.LicenseId != Id)
            throw new InvalidOperationException("Assignment does not belong to this license.");

        _assignments.Add(assignment);
        _reservationIds.Remove(assignment.UserId);
        AddDomainEvent(new LicenseAssignedEvent(Id, assignment.UserId));
    }

    public void Deactivate()
    {
        IsActive = false;
    }

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

    public void RemoveAssignment(Guid assignmentId)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId);
        if (assignment != null)
        {
            _assignments.Remove(assignment);
            AddDomainEvent(new AssignmentRemovedEvent(assignmentId, Id));
        }
    }
}

