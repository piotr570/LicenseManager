using MediatR;
using LicenseManager.Users.Application.Commands;
using LicenseManager.Users.Domain.Repositories;

namespace LicenseManager.Users.Application.Handlers;

public sealed class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");

        await userRepository.DeleteAsync(request.UserId, cancellationToken);
    }
}

