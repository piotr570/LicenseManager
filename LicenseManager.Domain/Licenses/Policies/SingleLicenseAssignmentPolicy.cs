using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Policies;

public class SingleLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void CanAssignUser(License license, User user)
    {
        if (license.AssignmentIds.Count > 0)
        {
            throw new PolicyViolationException($"Assignment policy denied for user {user.Id}.");
        }
    }
}