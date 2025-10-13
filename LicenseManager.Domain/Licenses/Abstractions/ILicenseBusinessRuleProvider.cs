using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Abstractions;

public interface ILicenseBusinessRuleProvider
{
    IEnumerable<IBusinessRule> GetRules(License license, User user);
    IEnumerable<IBusinessRule> GetDeletionRules(License license);
}