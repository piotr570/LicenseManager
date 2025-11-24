using LicenseManager.Users.Application.Commands;
using LicenseManager.Users.Domain.Aggregates;
using LicenseManager.Users.Domain.Repositories;
using MediatR;

namespace LicenseManager.Users.Application.Handlers;

public sealed class CreateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            throw new InvalidOperationException($"User with email '{request.Email}' already exists.");

        var user = new User(request.Email, request.Name, request.DepartmentId);
        await userRepository.AddAsync(user, cancellationToken);
        return user.Id;
    }
}