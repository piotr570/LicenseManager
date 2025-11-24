using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Rules;

public class TeamLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void CanAssignUser(int assignmentCount, int? maxUsers, DepartmentType? licenseDepartment, DepartmentType? userDepartment)
    {
        if ((licenseDepartment != null && userDepartment != licenseDepartment) 
            || assignmentCount >= maxUsers)
        {
            throw new PolicyViolationException("Team license assignment policy denied.");
        }
    }
}