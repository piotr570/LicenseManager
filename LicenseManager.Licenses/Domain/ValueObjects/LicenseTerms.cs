using LicenseManager.Licenses.Domain.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Licenses.Domain.ValueObjects;

public class LicenseTerms : ValueObject
{
    public int? MaxUsers { get; private set; }
    public int? UsageLimit { get; private set; }
    public LicenseType Type { get; private set; }
    public LicenseMode Mode { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public bool IsRenewable { get; private set; }
    public DateTime? RenewalDate { get; private set; }

    private LicenseTerms() { }

    public LicenseTerms(
        LicenseType type,
        LicenseMode mode,
        int? maxUsers,
        bool isRenewable,
        DateTime? expirationDate,
        DateTime? renewalDate,
        int? usageLimit)
    {
        ValidateBusinessRules(type, mode, maxUsers, isRenewable, expirationDate, renewalDate, usageLimit);

        MaxUsers = maxUsers;
        UsageLimit = usageLimit;
        Type = type;
        Mode = mode;
        ExpirationDate = expirationDate;
        IsRenewable = isRenewable;
        RenewalDate = renewalDate;
    }

    private static void ValidateBusinessRules(
        LicenseType type,
        LicenseMode mode,
        int? maxUsers,
        bool isRenewable,
        DateTime? expirationDate,
        DateTime? renewalDate,
        int? usageLimit)
    {
        // Single licenses cannot have multiple users
        if (type == LicenseType.Single && maxUsers.HasValue && maxUsers > 1)
            throw new InvalidOperationException("Single license cannot have more than one user.");

        // Team licenses must have at least 2 users
        if (type == LicenseType.Team && (!maxUsers.HasValue || maxUsers < 2))
            throw new InvalidOperationException("Team license must support at least 2 users.");

        // Server licenses must have at least 1 user
        if (type == LicenseType.Server && (!maxUsers.HasValue || maxUsers < 1))
            throw new InvalidOperationException("Server license must support at least 1 user.");

        // Usage-based licenses cannot have expiration date
        if (mode == LicenseMode.UsageBased && expirationDate.HasValue)
            throw new InvalidOperationException("Usage-based license cannot have expiration date.");

        // Usage-based licenses must have usage limit
        if (mode == LicenseMode.UsageBased && !usageLimit.HasValue)
            throw new InvalidOperationException("Usage-based license must have usage limit.");

        // Time-based licenses must have expiration date
        if (mode == LicenseMode.TimeBased && !expirationDate.HasValue)
            throw new InvalidOperationException("Time-based license must have expiration date.");

        // Subscription licenses must be renewable
        if (mode == LicenseMode.SubscriptionBased && !isRenewable)
            throw new InvalidOperationException("Subscription-based license must be renewable.");

        // Renewal date must be after expiration date
        if (expirationDate.HasValue && renewalDate.HasValue && renewalDate <= expirationDate)
            throw new InvalidOperationException("Renewal date must be after expiration date.");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MaxUsers;
        yield return UsageLimit;
        yield return Type;
        yield return Mode;
        yield return ExpirationDate;
        yield return IsRenewable;
        yield return RenewalDate;
    }
}

