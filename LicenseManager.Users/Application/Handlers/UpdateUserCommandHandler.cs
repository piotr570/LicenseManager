using LicenseManager.Users.Application.Commands;
using LicenseManager.Users.Domain.Repositories;
using MediatR;

namespace LicenseManager.Users.Application.Handlers;

public sealed class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found.");

        user.Update(request.Email, request.Name, request.DepartmentId);
        await userRepository.UpdateAsync(user, cancellationToken);
    }
}