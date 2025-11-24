using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Rules;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;
using System.Linq;

namespace LicenseManager.Domain.Reservations.Services;

public interface IReservationDomainService
{
    void CheckReservationRules(License license, Guid userId);
}

internal class ReservationDomainService : IReservationDomainService
{
    public void CheckReservationRules(License license, Guid userId)
    {
        var reservedUserIds = license.Assignments.Select(a => a.UserId);

        var rules = new List<IBusinessRule>
        {
            new LicenseMustBeActiveRule(license.IsActive),
            new UserMustNotAlreadyHaveLicenseRule(userId, reservedUserIds),
            new LicenseReservationLimitNotExceededRule(license)
        };

        foreach (var rule in rules)
        {
            if (rule.IsBroken())
                throw new BusinessRuleViolationException(rule);
        }
    }
}