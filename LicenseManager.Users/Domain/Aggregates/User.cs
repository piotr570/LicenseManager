using LicenseManager.Users.Domain.Events;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Users.Domain.Aggregates;

public sealed class User : Entity, IAggregateRoot
{
    public string Email { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid? DepartmentId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private User() { }

    public User(string email, string name, Guid? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (!IsValidEmail(email))
            throw new ArgumentException("Email format is invalid.", nameof(email));

        Email = email;
        Name = name;
        DepartmentId = departmentId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserCreatedEvent(Id, email, name, departmentId));
    }

    public void Update(string email, string name, Guid? departmentId)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (!IsValidEmail(email))
            throw new ArgumentException("Email format is invalid.", nameof(email));

        Email = email;
        Name = name;
        DepartmentId = departmentId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserUpdatedEvent(Id, email, name, departmentId));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserActivatedEvent(Id));
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

