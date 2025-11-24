using MediatR;

namespace LicenseManager.Users.Application.Commands;

public record CreateUserCommand(
    string Email,
    string Name,
    Guid? DepartmentId) : IRequest<Guid>;

public record UpdateUserCommand(
    Guid UserId,
    string Email,
    string Name,
    Guid? DepartmentId) : IRequest;

public record DeactivateUserCommand(Guid UserId) : IRequest;

public record ActivateUserCommand(Guid UserId) : IRequest;

public record DeleteUserCommand(Guid UserId) : IRequest;

