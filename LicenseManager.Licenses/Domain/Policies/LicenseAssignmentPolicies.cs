namespace LicenseManager.Licenses.Domain.Policies;

public interface ILicenseAssignmentPolicy
{
    void ValidateAssignment(
        int currentAssignmentCount,
        int? maxUsers,
        Guid? licenseDepartmentId = null,
        Guid? userDepartmentId = null);
}

public sealed class SingleLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void ValidateAssignment(int currentAssignmentCount, int? maxUsers, Guid? licenseDepartmentId = null, Guid? userDepartmentId = null)
    {
        if (currentAssignmentCount > 0)
            throw new InvalidOperationException("Single license can only be assigned to one user.");
    }
}

public sealed class TeamLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void ValidateAssignment(int currentAssignmentCount, int? maxUsers, Guid? licenseDepartmentId = null, Guid? userDepartmentId = null)
    {
        if (currentAssignmentCount >= maxUsers)
            throw new InvalidOperationException($"Team license cannot exceed {maxUsers} assignments.");
    }
}

public sealed class ServerLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void ValidateAssignment(int currentAssignmentCount, int? maxUsers, Guid? licenseDepartmentId = null, Guid? userDepartmentId = null)
    {
        if (licenseDepartmentId.HasValue && licenseDepartmentId != userDepartmentId)
            throw new InvalidOperationException("Server license can only be assigned to users in the same department.");
    }
}