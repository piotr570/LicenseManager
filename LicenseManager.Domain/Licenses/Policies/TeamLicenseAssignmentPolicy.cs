using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Policies;

public class TeamLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void CanAssignUser(License license, User user)
    {
        if (!((license.Department == null || user.Department == license.Department) 
            && license.AssignmentIds.Count < license.Terms.MaxUsers))
        {
            throw new PolicyViolationException($"Assignment policy denied for user {user.Id}.");
        }
    }
}