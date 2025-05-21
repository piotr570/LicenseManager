using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class InvokeLicenseCommandHandler(
    IRepository<License> licenseRepository,
    IRepository<User> userRepository,
    IUnitOfWork unitOfWork,
    ILogger<InvokeLicenseCommandHandler> logger)
    : IRequestHandler<InvokeLicenseCommand>
{
    public async Task Handle(InvokeLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Invoking a license with Id: {0} by user with Id: {1}", command.LicenseId, command.UserId);

        var license = await licenseRepository.GetByIdIncludingAsync(command.LicenseId, 
                          x => x.Terms,
                          x => x.Reservations)
                      ?? throw new NotFoundException(nameof(License), command.LicenseId);

        var user = await userRepository.GetByIdIncludingAsync(command.UserId, 
                       x => x.LicenseAssignments)
                   ?? throw new NotFoundException(nameof(User), command.UserId);
        
        license.Invoke(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully invoked a license with Id: {0} by user with Id: {1}", command.LicenseId,
            command.UserId);
    }
}