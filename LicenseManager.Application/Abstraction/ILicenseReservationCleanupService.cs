namespace LicenseManager.Application.Abstraction;

public interface ILicenseReservationCleanupService
{
    Task CleanupExpiredReservationsAsync(CancellationToken cancellationToken);
}