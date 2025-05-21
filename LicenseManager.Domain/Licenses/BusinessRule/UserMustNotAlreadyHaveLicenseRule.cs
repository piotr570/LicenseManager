using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class UserMustNotAlreadyHaveLicenseRule(License license, User user) : IBusinessRule
{
    public bool IsBroken() => license.Reservations.Any(r => r.UserId == user.Id);
    public string? Message => "User already has a reserved license.";
}