using LicenseManager.Application.UseCases.Users.Commands;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class DeleteUserCommandHandler(
    IRepository<User> repository,
    IUnitOfWork unitOfWork,
    IReadDbContext db,
    ILogger<DeleteUserCommandHandler> logger)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting a user with an id: {0}", command.Id);
        var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken)
                   ?? throw new NotFoundException(nameof(User));

        repository.Delete(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted a user with an id: {0}", command.Id);
    }
}