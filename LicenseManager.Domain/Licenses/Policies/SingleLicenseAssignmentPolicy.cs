using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Rules;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Policies;

public class SingleLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void CanAssignUser(int assignmentCount, int? maxUsers, DepartmentType? licenseDepartment = null, DepartmentType? userDepartment = null)
    {
        if (assignmentCount > 0)
            throw new PolicyViolationException("Single license can only be assigned to one user.");
    }
}