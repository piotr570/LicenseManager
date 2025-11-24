using LicenseManager.Application.UseCases.Users.Commands;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class UpdateUserCommandHandler(
    IReadDbContext db,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserCommandHandler> logger) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating user with Id: {0}", command.UserId);
        var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User));
        
        user.Update(command.Email, command.Name, command.Department);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Updated user with Id: {0}", command.UserId);
    }
}