using LicenseManager.Licenses.Domain.Enums;
using LicenseManager.Licenses.Domain.Repositories;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Licenses.Domain.Aggregates;

public sealed class Assignment : Entity
{
    public Guid LicenseId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AssignedAt { get; init; }
    public DateTime? LastInvokedAt { get; private set; }
    public AssignmentState State { get; private set; }

    private Assignment() { }

    public Assignment(Guid licenseId, Guid userId, DateTime assignedAt)
    {
        if (licenseId == Guid.Empty)
            throw new ArgumentException("License ID cannot be empty.", nameof(licenseId));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        LicenseId = licenseId;
        UserId = userId;
        AssignedAt = assignedAt;
        State = AssignmentState.Active;
    }

    public void UpdateLastActivity(DateTime now)
    {
        if (now < AssignedAt)
            throw new InvalidOperationException("Activity time cannot be earlier than the assignment time.");

        LastInvokedAt = now;
    }

    public void MarkAsNotUsed()
    {
        State = AssignmentState.NotUsed;
    }

    public void MarkAsExpired()
    {
        State = AssignmentState.Expired;
    }

    public void Revoke()
    {
        State = AssignmentState.Revoked;
    }
}