using AutoFixture;
using FluentAssertions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using LicenseManager.SharedKernel.Exceptions;
using Xunit;
using License = LicenseManager.Domain.Licenses.License;

namespace LicenseManager.Tests.Utils;

public abstract class TestBase
{
    protected static void AssertBrokenRule<TRule>(Func<object> action)
        where TRule : class, IBusinessRule
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(action);
        exception.BrokenRule.Should().BeOfType<TRule>();
    }
    
    protected static User CreateUser()
    {
        return new User("testuser@example.com", "name", DepartmentType.Board);
    }

    protected static License CreateLicense()
    {
        var fixture = new Fixture();

        var terms = new LicenseTerms(
            licenseId: Guid.NewGuid(),
            type: LicenseType.Server,
            mode: LicenseMode.TimeBased,
            maxUsers: 1,
            isRenewable: true,
            expirationDate: SystemClock.Now.AddMonths(6),
            renewalDate: null,
            usageLimit: null
        );

        return License.Create(Guid.NewGuid(), fixture.Create<string>(), fixture.Create<string>(), fixture.Create<string>(), terms);
    }
}