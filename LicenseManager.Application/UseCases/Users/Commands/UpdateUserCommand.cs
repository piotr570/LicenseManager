using LicenseManager.Domain.Users;
using MediatR;

namespace LicenseManager.Application.UseCases.Users.Commands;

public record UpdateUserCommand(Guid UserId, string Email, string Name, DepartmentType Department) : IRequest;