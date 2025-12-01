using LicenseManager.Licenses.Domain.Repositories;
using LicenseManager.SharedKernel.Common;

namespace LicenseManager.Licenses.Domain.Services;

public sealed class LicenseCleanupDomainService(ILicenseRepository licenseRepository)
    : ILicenseAssignmentCleanupDomainService, ILicenseReservationCleanupDomainService
{
    public async Task<int> CleanupUnusedAssignmentsAsync(int daysInactive, CancellationToken cancellationToken = default)
    {
        var licenses = await licenseRepository.GetAllAsync(cancellationToken);
        var cutoffDate = SystemClock.Now.AddDays(-daysInactive);
        var totalRemoved = 0;

        foreach (var license in licenses)
        {
            var removedCount = license.CleanupNotUsedAssignments(assignment =>
            {
                return assignment.LastInvokedAt == null || assignment.LastInvokedAt < cutoffDate;
            });

            if (removedCount > 0)
            {
                await licenseRepository.UpdateAsync(license, cancellationToken);
                totalRemoved += removedCount;
            }
        }

        return totalRemoved;
    }
    
    public async Task<int> CleanupExpiredReservationsAsync(int daysReserved, CancellationToken cancellationToken = default)
    {
        var licenses = await licenseRepository.GetAllAsync(cancellationToken);
        var cutoffDate = DateTime.UtcNow.AddDays(-daysReserved);
        int totalRemoved = 0;

        foreach (var license in licenses)
        {
            int removedCount = license.CleanupExpiredReservations(reservationId =>
            {
                return true; // Placeholder
            });

            if (removedCount > 0)
            {
                await licenseRepository.UpdateAsync(license, cancellationToken);
                totalRemoved += removedCount;
            }
        }

        return totalRemoved;
    }
}

