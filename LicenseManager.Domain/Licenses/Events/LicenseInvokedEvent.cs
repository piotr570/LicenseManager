using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Domain.Licenses.Events;

public class LicenseInvokedEvent(Guid licenseId, Guid userId) : DomainEvent
{
    public Guid LicenseId { get; } = licenseId;
    public Guid UserId { get; } = userId;
}