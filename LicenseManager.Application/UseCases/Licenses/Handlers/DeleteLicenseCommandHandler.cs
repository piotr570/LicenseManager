using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class DeleteLicenseCommandHandler(
    IRepository<License> repository,
    ILicenseBusinessRuleProvider businessRuleProvider,
    IUnitOfWork unitOfWork,
    ILogger<DeleteLicenseCommandHandler> logger)
    : IRequestHandler<DeleteLicenseCommand>
{
    public async Task Handle(DeleteLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting a license with Id: {0}", command.LicenseId);

        var license = await repository.GetByIdAsync(command.LicenseId, cancellationToken)
                      ?? throw new NotFoundException(nameof(License), command.LicenseId);

        var rules = businessRuleProvider.GetDeletionRules(license);
        license.CheckBusinessRules(rules);

        repository.Delete(license);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted a license with Id: {0}", command.LicenseId);
    }
}