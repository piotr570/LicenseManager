using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class UserMustBeAssignedToLicenseRule(License license, User user) : IBusinessRule
{
    public string Message => $"User {user.Id} is not assigned to the license.";

    public bool IsBroken() => license.Assignments.All(a => a.UserId != user.Id);
}