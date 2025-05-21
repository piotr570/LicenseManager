using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Common.BusinessRules;
using LicenseManager.Domain.Licenses;
using Assignment = LicenseManager.Domain.Assignments.Assignment;

namespace LicenseManager.Domain.Users;

public class User(string email, string name, DepartmentType department) : Entity
{
    private readonly List<Assignment> _licenseAssignments = [];
    private readonly List<LicenseReservation> _licenseReservations = [];

    public string Email { get; private set; } = email;
    public string Name { get; private set; } = name;
    public DepartmentType Department { get; private set; } = department;
    public IReadOnlyCollection<Assignment> LicenseAssignments => _licenseAssignments.AsReadOnly();
    public IReadOnlyCollection<LicenseReservation> LicenseReservations => _licenseReservations.AsReadOnly();

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