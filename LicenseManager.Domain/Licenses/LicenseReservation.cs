using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;

namespace LicenseManager.Domain.Licenses;

public class LicenseReservation : Entity, IAggregateRoot
{
    public Guid LicenseId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ReservedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private LicenseReservation() { }
    
    public LicenseReservation(Guid licenseId, Guid userId, DateTime? expiresAt = null)
    {
        if (licenseId == Guid.Empty) throw new ArgumentException("LicenseId cannot be empty");
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty");
        LicenseId = licenseId;
        UserId = userId;
        ReservedAt = SystemClock.Now;
        ExpiresAt = expiresAt ?? ReservedAt.AddDays(7);
    }

    public bool IsExpired(DateTime currentTime)
        => ExpiresAt.HasValue && ExpiresAt.Value <= currentTime;
}