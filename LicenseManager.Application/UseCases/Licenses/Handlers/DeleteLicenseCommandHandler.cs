using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Assignments;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.Domain.Licenses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class DeleteLicenseCommandHandler(
    IRepository<License> licenseRepository,
    IUnitOfWork unitOfWork,
    IReadDbContext db,
    ILogger<DeleteLicenseCommandHandler> logger)
    : IRequestHandler<DeleteLicenseCommand>
{
    public async Task Handle(DeleteLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting a license with Id: {0}", command.LicenseId);

        var license = await db.GetEntityOrThrowAsync<License>(command.LicenseId, cancellationToken);

        var licenseAssignmentExists = await db.Set<Assignment>().AnyAsync(x => x.LicenseId == command.LicenseId, cancellationToken);
        if (licenseAssignmentExists)
            throw new InvalidOperationException("Cannot delete a license that is currently assigned.");

        licenseRepository.Delete(license);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted a license with Id: {0}", command.LicenseId);
    }
}