using LicenseManager.Domain.Common.Events;

namespace LicenseManager.Domain.Licenses.Events;

public class LicenseReservationsCleanedUpEvent(Guid licenseId) : DomainEvent
{
    public Guid LicenseId { get; } = licenseId;
}
