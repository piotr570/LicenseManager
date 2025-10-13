using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class TeamLicenseMaxUsersRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.Type == LicenseType.Team && license.Assignments.Count >= license.Terms.MaxUsers;

    public string? Message => "The maximum number of users for this team license has been reached.";
}