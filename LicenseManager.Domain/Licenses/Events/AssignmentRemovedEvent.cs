using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Domain.Licenses.Events;

public class AssignmentRemovedEvent(Guid assignmentId, Guid licenseId) : DomainEvent
{
    public Guid AssignmentId { get; } = assignmentId;
    public Guid LicenseId { get; } = licenseId;
}
