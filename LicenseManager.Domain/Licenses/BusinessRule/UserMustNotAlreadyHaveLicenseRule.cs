using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class UserMustNotAlreadyHaveLicenseRule(Guid userId, IEnumerable<Guid> reservedUserIds) : IBusinessRule
{
    public bool IsBroken() => reservedUserIds.Contains(userId);
    public string Message => "User already has a reserved license.";
}