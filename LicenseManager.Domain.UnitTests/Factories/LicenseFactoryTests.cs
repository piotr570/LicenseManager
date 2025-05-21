using LicenseManager.Domain.Common;
using LicenseManager.Domain.Common.BusinessRules;
using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Licenses.Enums;
using LicenseManager.Domain.Licenses.Factories.Creation;
using LicenseManager.Domain.Licenses.Factories.Creation.BusinessRules;
using LicenseManager.Tests.Utils;
using Xunit;

namespace LicenseManager.UnitTests.Factories;

public class LicenseFactoryTests : TestBase
{
    private const string Key = "VALID-KEY-123";
    private const string Vendor = "VendorX";
    private const string Name = "LicenseX";
    private const LicenseType LicenseType = Domain.Licenses.Enums.LicenseType.Team;
    private const LicenseMode LicenseMode = Domain.Licenses.Enums.LicenseMode.TimeBased;
    private const int MaxUsers = 5;
    private const bool IsRenewable = true;
    private static readonly DateTime ExpirationDate = SystemClock.Now.AddYears(1);
    private static readonly DateTime RenewalDate = ExpirationDate.AddMonths(1);
    private readonly int? _usageLimit = null;

    private readonly LicenseFactory _licenseFactory = new();

    [Fact]
    public void Create_ShouldReturnLicense_WhenAllBusinessRulesPass()
    {
        // Arrange & Act
        var license = _licenseFactory.Create(Key, Vendor, Name, LicenseType, LicenseMode, MaxUsers, IsRenewable, ExpirationDate, RenewalDate, _usageLimit);

        // Assert
        Assert.NotNull(license);
        Assert.Equal(Key, license.Key);
        Assert.Equal(Vendor, license.Vendor);
        Assert.Equal(Name, license.Name);
        Assert.Equal(LicenseType, license.Terms.Type);
        Assert.Equal(LicenseMode, license.Terms.Mode);
        Assert.Equal(MaxUsers, license.Terms.MaxUsers);
        Assert.Equal(IsRenewable, license.Terms.IsRenewable);
        Assert.Equal(ExpirationDate, license.Terms.ExpirationDate);
        Assert.Equal(RenewalDate, license.Terms.RenewalDate);
        Assert.Equal(_usageLimit, license.Terms.UsageLimit);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenLicenseKeyIsEmpty()
    {
        // Arrange
        var invalidKey = string.Empty;

        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(invalidKey, Vendor, Name, LicenseType.Single, LicenseMode.TimeBased, 1, true, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNameIsEmpty()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, string.Empty, LicenseType.Single, LicenseMode.TimeBased, 1, true, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenVendorIsEmpty()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, string.Empty, Name, LicenseType.Single, LicenseMode.TimeBased, 1, true, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenSingleLicenseHasMoreThanOneUser()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Single, LicenseMode.TimeBased, 2, true, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenTeamLicenseHasLessThanTwoUsers()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Team, LicenseMode.TimeBased, 1, true, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenServerLicenseHasZeroUsers()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Server, LicenseMode.TimeBased, 0, true, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenUsageBasedLicenseHasNoUsageLimit()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Team, LicenseMode.UsageBased, 5, true, null, null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenTimeBasedLicenseHasNoExpirationDate()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Team, LicenseMode.TimeBased, 5, true, null, null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenSubscriptionLicenseIsNotRenewable()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Team, LicenseMode.SubscriptionBased, 5, false, SystemClock.Now.AddYears(1), null, null));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenRenewalDateIsBeforeExpirationDate()
    {
        // Act & Assert
        Assert.Throws<BusinessRuleViolationException>(() =>
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Team, LicenseMode.TimeBased, 5, true, SystemClock.Now.AddYears(1), SystemClock.Now, null));
    }
    
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrow_WhenLicenseKeyIsEmpty(string key)
    {
        AssertBrokenRule<LicenseKeyCannotBeEmptyRule>(() => 
            _licenseFactory.Create(key, Vendor, Name, 
                LicenseType.Single, LicenseMode.TimeBased, 1, true, 
                SystemClock.Now, null, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrow_WhenNameIsEmpty(string name)
    {
        AssertBrokenRule<ValueCannotBeEmptyRule>(() => 
            _licenseFactory.Create(Key, Vendor, name, 
                LicenseType.Single, LicenseMode.TimeBased, 1, true, 
                SystemClock.Now, null, null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldThrow_WhenVendorIsEmpty(string? vendor)
    {
        AssertBrokenRule<ValueCannotBeEmptyRule>(() => 
            _licenseFactory.Create(Key, vendor, Name, 
                LicenseType.Single, LicenseMode.TimeBased, 1, true, 
                SystemClock.Now, null, null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenSingleLicenseHasMultipleUsers()
    {
        AssertBrokenRule<SingleLicenseCannotHaveMultipleUsersRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, 
                LicenseType.Single, LicenseMode.TimeBased, 2, true,
                SystemClock.Now, null, null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTeamLicenseHasLessThanTwoUsers()
    {
        AssertBrokenRule<TeamLicenseMustHaveMoreThanOneUserRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, 
                LicenseType.Team, LicenseMode.TimeBased, 1, true, 
                SystemClock.Now, null, null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenServerLicenseHasZeroUsers()
    {
        AssertBrokenRule<ServerLicenseMustHaveAtLeastOneUserRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, 
                LicenseType.Server, LicenseMode.TimeBased, 0, true, 
                SystemClock.Now, null, null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenUsageBasedLicenseHasNoUsageLimit()
    {
        AssertBrokenRule<UsageBasedLicenseMustHaveUsageLimitRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, 
                LicenseType.Single, LicenseMode.UsageBased, 1, true, 
                null, null, null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenTimeBasedLicenseHasNoExpirationDate()
    {
        AssertBrokenRule<TimeBasedLicenseMustHaveExpirationDateRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Single, 
                LicenseMode.TimeBased, 1, true, 
                null, null, null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenSubscriptionLicenseIsNotRenewable()
    {
        AssertBrokenRule<SubscriptionLicenseMustBeRenewableRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Single, 
                LicenseMode.SubscriptionBased, 1, false, 
                SystemClock.Now, SystemClock.Now.AddMonths(1), null));
    }

    [Fact]
    public void Create_ShouldThrow_WhenRenewalDateIsBeforeExpiration()
    {
        AssertBrokenRule<RenewalDateMustBeAfterExpirationDateRule>(() => 
            _licenseFactory.Create(Key, Vendor, Name, LicenseType.Single, 
                LicenseMode.SubscriptionBased, 1, true, 
                SystemClock.Now.AddMonths(2), SystemClock.Now.AddMonths(1), null));
    }
}