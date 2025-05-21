using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Assignments;

public sealed class Assignment : Entity
{
    // EF Core only
    private Assignment()
    {
    }

    public Guid LicenseId { get; private set; }
    public Guid UserId { get; private set; }

    public License License { get; private set; } = null!;
    public User User { get; private set; } = null!;

    public DateTime AssignedAt { get; init; }
    public DateTime? LastInvokedAt { get; private set; }
    public AssignmentState State { get; private set; }

    public Assignment(License license, User user, DateTime assignedAt)
    {
        License = license ?? throw new ArgumentNullException(nameof(license));
        User = user ?? throw new ArgumentNullException(nameof(user));

        LicenseId = license.Id;
        UserId = user.Id;

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