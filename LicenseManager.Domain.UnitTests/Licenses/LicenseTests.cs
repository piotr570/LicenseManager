using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Events;
using LicenseManager.Domain.Licenses.Policies;
using LicenseManager.Domain.Users;
using Moq;
using Xunit;
using FluentAssertions;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Licenses.BusinessRule;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Factories.Assignment;
using LicenseManager.Tests.Utils;
using License = LicenseManager.Domain.Licenses.License;

namespace LicenseManager.UnitTests.Licenses;

public class LicenseTests : TestBase
{
    private readonly Mock<ILicenseAssignmentPolicyFactory> _policyFactoryMock;
    private readonly Mock<ILicenseAssignmentPolicy> _policyMock;
    private readonly Mock<IBusinessRule> _businessRuleMock;
    private readonly License _license;
    private readonly User _user;

    public LicenseTests()
    {
        _policyFactoryMock = new Mock<ILicenseAssignmentPolicyFactory>();
        _policyMock = new Mock<ILicenseAssignmentPolicy>();
        _businessRuleMock = new Mock<IBusinessRule>();

        _user = new User("testuser@example.com", "name", DepartmentType.Board);

        var terms = new LicenseTerms(
            licenseId: Guid.NewGuid(),
            type: LicenseType.Single,
            mode: LicenseMode.TimeBased,
            maxUsers: 1,
            isRenewable: true,
            expirationDate: SystemClock.Now.AddMonths(6),
            renewalDate: null,
            usageLimit: null
        );
        
        _license = License.Create(Guid.NewGuid(), "KEY123", "VendorX", "LicenseX", terms);
    }

    [Fact]
    public void AssignUser_ShouldAssignUser_WhenPolicyAllowsAndRulesPass()
    {
        // Arrange
        _policyMock.Setup(p => p.CanAssignUser(_license, _user));
        _policyFactoryMock.Setup(f => f.GetPolicy(_license.Terms.Type)).Returns(_policyMock.Object);

        // Act
        _license.AssignUser(_user, _policyMock.Object, new List<IBusinessRule>(), SystemClock.Now);

        // Assert
        Assert.Contains(_license.Assignments, a => a.User.Equals(_user));
    }

    [Fact]
    public void AssignUser_ShouldThrowException_WhenPolicyDeniesAssignment()
    {
        // Arrange
        _policyMock.Setup(p => p.CanAssignUser(_license, _user)).Throws(() => new PolicyViolationException(string.Empty));
        _policyFactoryMock.Setup(f => f.GetPolicy(_license.Terms.Type)).Returns(_policyMock.Object);

        // Act & Assert
        Assert.Throws<PolicyViolationException>(() => _license.AssignUser(_user, _policyMock.Object, new List<IBusinessRule>(), SystemClock.Now));
    }

    [Fact]
    public void AssignUser_ShouldThrowException_WhenBusinessRuleFails()
    {
        // Arrange
        _policyMock.Setup(p => p.CanAssignUser(_license, _user));
        _policyFactoryMock.Setup(f => f.GetPolicy(_license.Terms.Type)).Returns(_policyMock.Object);

        _businessRuleMock.Setup(r => r.IsBroken()).Returns(true);
        var rules = new List<IBusinessRule> { _businessRuleMock.Object };

        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() => _license.AssignUser(_user, _policyMock.Object, rules, SystemClock.Now));
    }
    
    [Fact]
    public void Invoke_Should_ThrowException_When_LicenseIsNotActive()
    {
        // Arrange
        var license = CreateLicense(isActive: false);
        var user = CreateUser();
    
        // Act
        Assert.Throws<LicenseNotActiveException>(() => license.Invoke(user));
    }

    [Fact]
    public void Invoke_Should_IncrementUsageCount_When_BusinessRulesPass()
    {
        // Arrange
        var license = CreateLicense(isActive: true);
        var user = CreateUser();
        _policyMock.Setup(p => p.CanAssignUser(license, user));
        license.AssignUser(user, _policyMock.Object, new List<IBusinessRule>(), SystemClock.Now);
        
        // Act
        license.Invoke(user);
    
        // Assert
        license.UsageCount.Should().Be(1);
        license.Assignments.First().LastInvokedAt.Should().NotBe(null);
        license.DomainEvents.Should().ContainSingle(e => e is LicenseInvokedEvent);
    }
    
    [Fact]
    public void Reserve_ShouldReserveLicense_WhenBusinessRulesPass()
    {
        // Arrange
        var user = CreateUser();
    
        var license = CreateLicense();

        // Act
        license.Reserve(user);

        // Assert
        var reservation = license.Reservations.SingleOrDefault(r => r.User.Equals(user));
        reservation.Should().NotBeNull();
        license.DomainEvents.Should().ContainSingle(e => e is LicenseReservedEvent);
    }
    
    [Fact]
    public void CleanupExpiredReservations_ShouldRemoveExpiredReservations_WhenReservationsHaveExpired()
    {
        // Arrange
        var user = CreateUser();
        var expiredDate = SystemClock.Now.AddDays(-10);

        SystemClock.Set(expiredDate);
        var reservation = new LicenseReservation(_license, user);
        _license.Reserve(user);
        SystemClock.Reset();

        // Act
        var removedCount = _license.CleanupExpiredReservations();

        // Assert
        removedCount.Should().BeGreaterThan(0);
        _license.Reservations.Should().NotContain(reservation);
    }
    
    [Fact]
    public void AssignUser_ShouldRemoveExpiredReservation_WhenUserAlreadyHasReservation()
    {
        // Arrange
        var user = CreateUser();
        _license.Reserve(user); // Ensure reservation is expired
        _license.CleanupExpiredReservations();
    
        var policyMock = new Mock<ILicenseAssignmentPolicy>();
        policyMock.Setup(p => p.CanAssignUser(_license, user));
    
        // Act
        _license.AssignUser(user, policyMock.Object, new List<IBusinessRule>(), SystemClock.Now);
    
        // Assert
        _license.Reservations.Should().BeEmpty();
    }

    [Fact]
    public void CleanupExpiredReservations_ShouldReturnZero_WhenNoExpiredReservationsExist()
    {
        // Arrange
        var user = CreateUser();
        var license = CreateLicense();
        license.Reserve(user);
    
        // Act
        var removedCount = license.CleanupExpiredReservations();
    
        // Assert
        removedCount.Should().Be(0); // No expired reservations
    }

    private License CreateLicense(bool isActive = true)
    {
        var terms = new LicenseTerms(
            Guid.NewGuid(),
            LicenseType.Single,
            LicenseMode.UsageBased,
            maxUsers: 1,
            isRenewable: true,
            expirationDate: SystemClock.Now.AddDays(10),
            renewalDate: null,
            usageLimit: 10);

        var license = License.Create(
            Guid.NewGuid(),
            "LICENSE-KEY",
            "Vendor",
            "Product Name",
            terms);

        if (!isActive)
        {
            license.Deactivate();
        }

        return license;
    }
}