using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class CannotDeleteAssignedLicenseRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Assignments.Count != 0;
    public string? Message => "A license with active assignments cannot be deleted.";
}
