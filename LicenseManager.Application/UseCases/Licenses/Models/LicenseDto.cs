using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Application.UseCases.Licenses.Models;

public record LicenseDto
{
    internal LicenseDto(License license, int assignmentsCount, bool isExpiringSoon = false)
    {
        Id = license.Id;
        Name = license.Name;
        Key = license.Key;
        Vendor = license.Vendor;
        IsActive = license.IsActive;
        IsCancelled = license.IsCancelled;
        AssignmentsCount = assignmentsCount;
        IsExpiringSoon = isExpiringSoon;
        Terms = new LicenseTermsDto(license.Terms);
    }

    public Guid Id { get; }
    public string Key { get; }
    public string Name { get; }
    public string Vendor { get; }
    public bool IsActive { get; }
    public bool IsCancelled { get; }
    public int AssignmentsCount { get; }
    public bool IsExpiringSoon { get; }
    public LicenseTermsDto Terms { get; }

    public class LicenseTermsDto(LicenseTerms licenseTerms)
    {
        public DateTime? ExpirationDate { get; } = licenseTerms.ExpirationDate;
        public DateTime? RenewalDate { get; } = licenseTerms.RenewalDate;
        public bool IsRenewable { get; } = licenseTerms.IsRenewable;
        public LicenseMode Mode { get; } = licenseTerms.Mode;
        public LicenseType Type { get; } = licenseTerms.Type;
        public int? MaxUsers { get; } = licenseTerms.MaxUsers;
        public int? UsageLimit { get; } = licenseTerms.UsageLimit;
    }
}