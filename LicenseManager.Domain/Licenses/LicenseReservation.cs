using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses;

public class LicenseReservation : Entity
{
    public Guid LicenseId { get; private set; }
    public License License { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public DateTime ReservedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }

    // Private Constructor (For EF Core)
    private LicenseReservation() { }

    // Factory Constructor for Internal Domain Use
    internal LicenseReservation(License license, User user)
    {
        License = license ?? throw new ArgumentNullException(nameof(license));
        LicenseId = license.Id;

        User = user ?? throw new ArgumentNullException(nameof(user));
        UserId = user.Id;

        ReservedAt = SystemClock.Now;
        ExpiresAt = ReservedAt.AddDays(7);
    }

    public bool IsExpired(DateTime currentTime)
        => ExpiresAt.HasValue && ExpiresAt.Value <= currentTime;
}