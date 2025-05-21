namespace LicenseManager.Application.HostedServices.Interfaces;

public interface ILicenseReservationCleanupService
{
    Task CleanupExpiredReservationsAsync(CancellationToken cancellationToken);
}