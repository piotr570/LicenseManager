using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;

namespace LicenseManager.Domain.Assignments;

public sealed class Assignment : Entity
{
    public Guid LicenseId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AssignedAt { get; init; }
    public DateTime? LastInvokedAt { get; private set; }
    public AssignmentState State { get; private set; }

    // For EF Core
    private Assignment() { }

    public Assignment(Guid licenseId, Guid userId, DateTime assignedAt)
    {
        if (licenseId == Guid.Empty) throw new ArgumentNullException(nameof(licenseId));
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
        LicenseId = licenseId;
        UserId = userId;
        AssignedAt = assignedAt;
        State = AssignmentState.Active;
    }

    public void UpdateLastActivity()
    {
        var now = SystemClock.Now;
        if (now < AssignedAt)
        {
            throw new ArgumentException("Activity time cannot be earlier than the assignment time.");
        }

        LastInvokedAt = now;
    }
    
    public void MarkAsNotUsed()
    {
        State = AssignmentState.NotUsed;
    }
}