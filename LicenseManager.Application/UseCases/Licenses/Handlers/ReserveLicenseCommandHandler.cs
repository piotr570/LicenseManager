using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Reservations;
using LicenseManager.Domain.Reservations.Services;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using LicenseManager.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class ReserveLicenseCommandHandler(
    IRepository<LicenseReservation> licenseReservationRepository,
    IReservationDomainService reservationDomainService,
    IReadDbContext db,
    IUnitOfWork unitOfWork,
    ILogger<DeleteLicenseCommandHandler> logger) : IRequestHandler<ReserveLicenseCommand>
{
    public async Task Handle(ReserveLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Reserving a license with Id: {0} for user with Id: {1}", command.LicenseId, command.UserId);

        var license = await db.GetEntityOrThrowAsync<License>(command.LicenseId, cancellationToken);
        var user = await db.GetEntityOrThrowAsync<User>(command.UserId, cancellationToken);
        
        reservationDomainService.CheckReservationRules(license, user.Id);
        
        var now = SystemClock.Now;
        var licenseReservation = new LicenseReservation(license.Id, user.Id, now, now.AddDays(7));

        await licenseReservationRepository.AddAsync(licenseReservation);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully reserved a license with Id: {0} for user with Id: {1}", command.LicenseId, command.UserId);
    }
}
