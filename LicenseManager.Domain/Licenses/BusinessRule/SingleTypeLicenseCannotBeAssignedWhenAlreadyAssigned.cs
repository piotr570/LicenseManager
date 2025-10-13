using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class SingleTypeLicenseCannotBeAssignedWhenAlreadyAssigned(License license, User user) : IBusinessRule
{
    public bool IsBroken() => license.Assignments.Count > 0 && license.Assignments.All(a => a.UserId != user.Id);

    public string? Message => "A Single User License cannot be reassigned to a different user.";
}