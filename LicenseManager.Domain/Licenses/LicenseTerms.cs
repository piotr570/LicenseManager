using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses;

public class LicenseTerms : Entity
{
    public Guid LicenseId { get; private set; }
    public License License { get; private set; } = null!;
    public int? MaxUsers { get; private set; }
    public int? UsageLimit { get; private set; }
    public LicenseType Type { get; private set; }
    public LicenseMode Mode { get; private set; }
    public DateTime? ExpirationDate { get; private set; }
    public bool IsRenewable { get; private set; } 
    public DateTime? RenewalDate { get; private set; }
    
    // Private Constructor (For EF Core)
    private LicenseTerms() { }
    
    internal LicenseTerms(Guid licenseId, 
        LicenseType type, 
        LicenseMode mode, 
        int? maxUsers, 
        bool isRenewable, 
        DateTime? expirationDate, 
        DateTime? renewalDate, 
        int? usageLimit)
    {
        Id = licenseId;
        LicenseId = licenseId;
        MaxUsers = maxUsers;
        UsageLimit = usageLimit;
        Type = type;
        Mode = mode;
        ExpirationDate = expirationDate;
        IsRenewable = isRenewable;
        RenewalDate = renewalDate;
    }
}