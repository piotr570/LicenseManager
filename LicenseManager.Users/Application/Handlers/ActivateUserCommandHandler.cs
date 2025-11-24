using LicenseManager.Users.Application.Commands;
using LicenseManager.Users.Domain.Repositories;
using MediatR;

namespace LicenseManager.Users.Application.Handlers;

public sealed class ActivateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<ActivateUserCommand>
{
    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");

        user.Activate();
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}