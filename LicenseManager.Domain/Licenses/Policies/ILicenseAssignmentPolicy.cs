using LicenseManager.Domain.Users;

namespace LicenseManager.Domain.Licenses.Policies;

public interface ILicenseAssignmentPolicy
{
    void CanAssignUser(License license, User user);
}