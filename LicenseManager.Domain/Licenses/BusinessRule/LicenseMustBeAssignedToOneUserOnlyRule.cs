using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseMustBeAssignedToOneUserOnlyRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.Type == LicenseType.Single && license.Assignments.Count > 1;

    public string? Message => "Single-user licenses can only be assigned to one user.";
}