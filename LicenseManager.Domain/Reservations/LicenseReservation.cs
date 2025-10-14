using LicenseManager.Domain.Reservations.Events;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Reservations;

public enum ReservationState
{
    Active,
    Expired,
    Cancelled
}

public class LicenseReservation : Entity, IAggregateRoot
{
    public Guid LicenseId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ReservedAt { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public ReservationState State { get; private set; }

    // For EF Core
    private LicenseReservation() { }

    public LicenseReservation(Guid licenseId, 
        Guid userId, 
        DateTime reservedAt, 
        DateTime? expirationDate)
    {
        if (licenseId == Guid.Empty) throw new ArgumentException("LicenseId cannot be empty");
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty");
        LicenseId = licenseId;
        UserId = userId;
        ReservedAt = reservedAt;
        ExpirationDate = expirationDate;
        State = ReservationState.Active;
        AddDomainEvent(new LicenseReservedEvent(licenseId, userId, reservedAt));
    }

    public void Expire()
    {
        if (State != ReservationState.Active) return;
        State = ReservationState.Expired;
    }

    public void Cancel()
    {
        if (State != ReservationState.Active) return;
        State = ReservationState.Cancelled;
    }

    public bool IsExpired(DateTime now)
    {
        return ExpirationDate.HasValue && ExpirationDate.Value <= now || State == ReservationState.Expired;
    }
}