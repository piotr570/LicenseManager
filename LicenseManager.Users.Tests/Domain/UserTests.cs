using LicenseManager.Users.Domain.Aggregates;
using Xunit;

namespace LicenseManager.Users.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldCreateUser_WithValidParameters()
    {
        // Arrange
        var email = "user@example.com";
        var name = "John Doe";
        var departmentId = Guid.NewGuid();

        // Act
        var user = new User(email, name, departmentId);

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.Equal(departmentId, user.DepartmentId);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEmailIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User("", "John Doe"));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenEmailIsInvalid()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User("invalid-email", "John Doe"));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            new User("user@example.com", ""));
    }

    [Fact]
    public void Update_ShouldModifyUserProperties()
    {
        // Arrange
        var user = new User("user@example.com", "John Doe");
        var newEmail = "newemail@example.com";
        var newName = "Jane Doe";

        // Act
        user.Update(newEmail, newName, null);

        // Assert
        Assert.Equal(newEmail, user.Email);
        Assert.Equal(newName, user.Name);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var user = new User("user@example.com", "John Doe");
        Assert.True(user.IsActive);

        // Act
        user.Deactivate();

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        // Arrange
        var user = new User("user@example.com", "John Doe");
        user.Deactivate();
        Assert.False(user.IsActive);

        // Act
        user.Activate();

        // Assert
        Assert.True(user.IsActive);
    }
}

