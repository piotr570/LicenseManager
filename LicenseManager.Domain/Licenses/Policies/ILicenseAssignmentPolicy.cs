namespace LicenseManager.Domain.Licenses.Policies;

public interface ILicenseAssignmentPolicy
{
    void CanAssignUser(Guid licenseId, Guid userId);
}