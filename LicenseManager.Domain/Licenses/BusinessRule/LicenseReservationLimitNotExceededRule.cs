using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Licenses.Rules;

public class LicenseReservationLimitNotExceededRule(License license) : IBusinessRule
{
    public bool IsBroken() => license.ReservationIds.Count >= license.Terms.MaxUsers;
    public string? Message => "License reservation limit exceeded.";
}