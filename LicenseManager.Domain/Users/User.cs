using LicenseManager.SharedKernel.BusinessRules;
using LicenseManager.SharedKernel.Abstractions;

namespace LicenseManager.Domain.Users;

public class User : Entity, IAggregateRoot
{
    public string Email { get; private set; }
    public string Name { get; private set; }
    public DepartmentType Department { get; private set; }

    private User() { }
    
    public User(string email, string name, DepartmentType department)
    {
        CheckBusinessRules([
            new ValueCannotBeEmptyRule(nameof(Email), email),
            new ValueCannotBeEmptyRule(nameof(Name), name)]);
        Email = email;
        Name = name;
        Department = department;
    }

    public void Update(string email, string name, DepartmentType department)
    {
        CheckBusinessRules([
            new ValueCannotBeEmptyRule(nameof(Email), email),
            new ValueCannotBeEmptyRule(nameof(Name), name)]);

        Email = email;
        Name = name;
        Department = department;
    }
}