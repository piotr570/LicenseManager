using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class InvokeLicenseCommandHandler(
    IReadDbContext db,
    IUnitOfWork unitOfWork,
    ILogger<InvokeLicenseCommandHandler> logger)
    : IRequestHandler<InvokeLicenseCommand>
{
    public async Task Handle(InvokeLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Invoking a license with Id: {0} by user with Id: {1}", command.LicenseId, command.UserId);

        var license = await db.GetEntityOrThrowAsync<License>(command.LicenseId, cancellationToken);
        var user = await db.GetEntityOrThrowAsync<User>(command.UserId, cancellationToken);
        
        license.Invoke(user.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully invoked a license with Id: {0} by user with Id: {1}", command.LicenseId, command.UserId);
    }
}