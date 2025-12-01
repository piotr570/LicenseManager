using LicenseManager.Licenses.Domain.Services;

namespace LicenseManager.Licenses.Application.Services;

public interface ILicenseAssignmentCleanupService
{
    Task<int> CleanupUnusedAssignmentsAsync(CancellationToken cancellationToken = default);
}

public interface ILicenseReservationCleanupService
{
    Task<int> CleanupExpiredReservationsAsync(CancellationToken cancellationToken = default);
}

internal sealed class LicenseAssignmentCleanupService(
    ILicenseAssignmentCleanupDomainService domainService,
    int daysInactive = 30)
    : ILicenseAssignmentCleanupService
{
    public async Task<int> CleanupUnusedAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        return await domainService.CleanupUnusedAssignmentsAsync(daysInactive, cancellationToken);
    }
}

internal sealed class LicenseReservationCleanupService(
    ILicenseReservationCleanupDomainService domainService,
    int daysReserved = 7)
    : ILicenseReservationCleanupService
{
    public async Task<int> CleanupExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        return await domainService.CleanupExpiredReservationsAsync(daysReserved, cancellationToken);
    }
}

