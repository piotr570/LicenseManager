using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Rules;

public interface ILicenseAssignmentPolicy
{
    void CanAssignUser(int assignmentCount, int? maxUsers, DepartmentType? licenseDepartment = null, DepartmentType? userDepartment = null);
}