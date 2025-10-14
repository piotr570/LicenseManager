using LicenseManager.Application.HostedServices.Interfaces;
using LicenseManager.Domain.Licenses;
using LicenseManager.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.HostedServices;

public class LicenseReservationCleanupService(
    IRepository<License> repository,
    IUnitOfWork unitOfWork,
    ILogger<LicenseReservationCleanupService> logger)
    : ILicenseReservationCleanupService
{
    public async Task CleanupExpiredReservationsAsync(CancellationToken cancellationToken)
    {
        // logger.LogInformation("Running License Reservation Cleanup Service...");
        //
        // var licensesWithReservations = await repository
        //     .GetAllIncludingAsync(null,query => query.Include(l => l.Reservations));
        //
        // foreach (var license in licensesWithReservations)
        // {
        //     license.CleanupExpiredReservations();
        //
        //     if (license.DomainEvents.Any())
        //     {
        //         repository.Update(license);
        //     }
        // }
        //
        // await unitOfWork.SaveChangesAsync(cancellationToken);
        //
        // logger.LogInformation("License Reservation Cleanup completed.");
    }
}