using LicenseManager.Domain.Common;
using LicenseManager.Domain.Users;

namespace LicenseManager.Application.UseCases.Users.Models;

public record UserDto(Guid Id, string Email, string Name, DepartmentType Department)
{
    internal UserDto(User user) : this(user.Id, user.Email, user.Name, user.Department)
    {
    }
}