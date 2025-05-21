using LicenseManager.Domain.Common;
using LicenseManager.Domain.Users;
using MediatR;

namespace LicenseManager.Application.UseCases.Users.Commands;

public record CreateUserCommand(string Email, string Name, DepartmentType Department) : IRequest<Guid>;