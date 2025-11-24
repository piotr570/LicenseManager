using LicenseManager.Users.Application.Commands;
using LicenseManager.Users.Domain.Repositories;
using MediatR;

namespace LicenseManager.Users.Application.Handlers;

public sealed class DeactivateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeactivateUserCommand>
{
    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");

        user.Deactivate();
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}