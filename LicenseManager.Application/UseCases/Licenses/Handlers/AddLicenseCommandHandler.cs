using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Factories.Creation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class AddLicenseCommandHandler(
    IRepository<License> repository,
    IUnitOfWork unitOfWork, 
    ILicenseFactory licenseFactory,
    ILogger<AddLicenseCommandHandler> logger) 
    : IRequestHandler<AddLicenseCommand, Guid>
{
    public async Task<Guid> Handle(AddLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding new license Name: {0} with Key: {1}.", command.Name, command.Key);

        var license = licenseFactory.Create(
            command.Key, 
            command.Vendor, 
            command.Name, 
            command.LicenseType, 
            command.LicenseMode, 
            command.MaxUsers, 
            command.IsRenewable, 
            command.ExpirationDate, 
            command.RenewalDate, 
            command.UsageLimit
        );

        await repository.AddAsync(license);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully added a new license with Name: {0} and Id: {1}.", command.Name, license.Id);
        
        return license.Id;
    }
}