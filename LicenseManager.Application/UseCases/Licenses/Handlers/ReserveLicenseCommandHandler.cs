using LicenseManager.Application.Abstraction;
using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class ReserveLicenseCommandHandler(
    IRepository<License> licenseRepository,
    IRepository<User> userRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteLicenseCommandHandler> logger) : IRequestHandler<ReserveLicenseCommand>
{
    public async Task Handle(ReserveLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reserving a license with Id: {0} for user with Id: {1}", command.LicenseId, command.UserId);

        var license = await licenseRepository.GetByIdIncludingAsync(command.LicenseId, 
                          x => x.Terms,
                          x => x.Reservations,
                          x => x.Assignments)
                      ?? throw new NotFoundException(nameof(License), command.LicenseId);

        var user = await userRepository.GetByIdIncludingAsync(command.UserId, 
                       x => x.LicenseAssignments,
                       x => x.LicenseReservations)
                   ?? throw new NotFoundException(nameof(User), command.UserId);

        license.Reserve(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully reserved a license with Id: {0} for user with Id: {1}", command.LicenseId, command.UserId);
    }
}
