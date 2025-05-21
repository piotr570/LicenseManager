using MediatR;

namespace LicenseManager.Application.UseCases.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest;