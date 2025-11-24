namespace LicenseManager.Infrastructure.Options;

public class LicenseReservationCleanupOptions
{
    public int CleanupFrequencyInHours { get; init; } = 1;
}