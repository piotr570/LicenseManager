using FluentAssertions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Users;
using Xunit;

namespace LicenseManager.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Constructor_Should_Initialize_Properties()
    {
        // Arrange
        var email = "testuser@example.com";
        var name = "John Doe";
        var department = DepartmentType.Board;

        // Act
        var user = new User(email, name, department);

        // Assert
        user.Email.Should().Be(email);
        user.Name.Should().Be(name);
        user.Department.Should().Be(department);
        user.LicenseAssignments.Should().BeEmpty();
    }

    [Fact]
    public void Update_Should_Update_Properties_When_Valid_Input()
    {
        // Arrange
        var user = new User("oldemail@example.com", "Old Name", DepartmentType.Board);

        var email = "newemail@example.com";
        var name = "New Name";
        var department = DepartmentType.Business;

        // Act
        user.Update(email, name, department);

        // Assert
        user.Email.Should().Be(email);
        user.Name.Should().Be(name);
        user.Department.Should().Be(department);
    }

    [Fact]
    public void Update_Should_Throw_Exception_When_Email_Is_Empty()
    {
        // Arrange
        var user = new User("validemail@example.com", "Valid Name", DepartmentType.Board);

        // Act
        Action act = () => user.Update(string.Empty, "Valid Name", DepartmentType.Board);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("Email cannot be empty.");
    }

    [Fact]
    public void Update_Should_Throw_Exception_When_Name_Is_Empty()
    {
        // Arrange
        var user = new User("validemail@example.com", "Valid Name", DepartmentType.Board);

        // Act
        Action act = () => user.Update("validemail@example.com", string.Empty, DepartmentType.Board);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("Name cannot be empty.");
    }

    [Fact]
    public void Update_Should_Throw_Exception_When_Email_Is_Null()
    {
        // Arrange
        var user = new User("validemail@example.com", "Valid Name", DepartmentType.Board);

        // Act
        Action act = () => user.Update(null, "Valid Name", DepartmentType.Board);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("Email cannot be empty.");
    }

    [Fact]
    public void Update_Should_Throw_Exception_When_Name_Is_Null()
    {
        // Arrange
        var user = new User("validemail@example.com", "Valid Name", DepartmentType.Board);

        // Act
        Action act = () => user.Update("validemail@example.com", null, DepartmentType.Board);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("Name cannot be empty.");
    }
}
