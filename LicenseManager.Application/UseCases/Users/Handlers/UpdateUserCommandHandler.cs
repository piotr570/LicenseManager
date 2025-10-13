using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Users.Commands;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class UpdateUserCommandHandler(
    IRepository<User> repository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user with Id: {0}", command.UserId);
        var user = await repository.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), command.UserId);
        
        user.Update(command.Email, command.Name, command.Department);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Updated user with Id: {0}", command.UserId);
    }
}