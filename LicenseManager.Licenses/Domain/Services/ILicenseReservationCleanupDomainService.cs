namespace LicenseManager.Licenses.Domain.Services;

internal interface ILicenseReservationCleanupDomainService
{
    Task<int> CleanupExpiredReservationsAsync(int daysReserved, CancellationToken cancellationToken = default);
}