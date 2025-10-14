using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Policies;

public class ServerLicenseAssignmentPolicy : ILicenseAssignmentPolicy
{
    public void CanAssignUser(License license, User user)
    {
        if (license.AssignmentIds.Count > license.Terms.MaxUsers)
        {
            throw new PolicyViolationException($"Assignment policy denied for user {user.Id}.");
        }
    }
}