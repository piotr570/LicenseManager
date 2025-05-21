using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Domain.Licenses.Factories.Creation;

public interface ILicenseFactory
{
    License Create(string key, string vendor, string name, LicenseType licenseType,
        LicenseMode licenseMode, int? maxUsers, bool isRenewable, DateTime? expirationDate,
        DateTime? renewalDate, int? usageLimit);
}