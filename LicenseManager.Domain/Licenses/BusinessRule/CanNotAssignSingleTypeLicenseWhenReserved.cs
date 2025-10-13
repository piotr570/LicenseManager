using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class CanNotAssignSingleTypeLicenseWhenReserved(License license) : IBusinessRule
{
    public bool IsBroken() => license.Terms.Type == LicenseType.Single 
                              && license.Reservations.Count > 0;

    public string? Message => "Cannot assign user to license: license is reserved.";
}