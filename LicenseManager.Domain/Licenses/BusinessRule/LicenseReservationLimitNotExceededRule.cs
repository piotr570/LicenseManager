using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.BusinessRule;

public class LicenseReservationLimitNotExceededRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.Reservations.Count >= license.Terms.MaxUsers;
    public string? Message => "License reservation limit exceeded.";
}