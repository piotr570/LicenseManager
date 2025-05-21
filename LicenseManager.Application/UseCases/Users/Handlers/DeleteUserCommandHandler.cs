using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Users.Commands;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class DeleteUserCommandHandler(
    IRepository<User> repository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteUserCommandHandler> logger)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting a user with an id: {0}", command.Id);
        var user = await repository.GetByIdAsync(command.Id, cancellationToken)
                   ?? throw new NotFoundException(nameof(User), command.Id);;

        repository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted a user with an id: {0}", command.Id);
    }
}