using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Rules;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Policies;

public class ServerLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void CanAssignUser(int assignmentCount, int? maxUsers, DepartmentType? licenseDepartment = null, DepartmentType? userDepartment = null)
    {
        if (assignmentCount >= maxUsers)
            throw new PolicyViolationException("Server license assignment policy denied.");
    }
}