using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Abstractions;

public interface ILicenseBusinessRuleProvider
{
    IEnumerable<IBusinessRule> GetRules(License license, User user);
    IEnumerable<IBusinessRule> GetDeletionRules(License license);
}