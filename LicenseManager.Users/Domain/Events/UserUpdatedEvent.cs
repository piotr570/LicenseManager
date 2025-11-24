using LicenseManager.SharedKernel.Events;

namespace LicenseManager.Users.Domain.Events;

public sealed record UserUpdatedEvent(
    Guid UserId,
    string Email,
    string Name,
    Guid? DepartmentId) : IDomainEvent
{
    public Guid AggregateId => UserId;
    public DateTime OccurredOn => DateTime.UtcNow;
}