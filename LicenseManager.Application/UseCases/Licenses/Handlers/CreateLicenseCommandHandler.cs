using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Licenses;
using LicenseManager.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class CreateLicenseCommandHandler(
    IRepository<License> repository,
    IUnitOfWork unitOfWork, 
    ILogger<CreateLicenseCommandHandler> logger) 
    : IRequestHandler<CreateLicenseCommand, Guid>
{
    public async Task<Guid> Handle(CreateLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding new license Name: {0} with Key: {1}.", command.Name, command.Key);

        var terms = new LicenseTerms(command.Terms.LicenseType, 
            command.Terms.LicenseMode,
            command.Terms.MaxUsers,
            command.Terms.IsRenewable,
            command.Terms.ExpirationDate,
            command.Terms.RenewalDate,
            command.Terms.UsageLimit);
        
        var license = new License(
            command.Key, 
            command.Vendor, 
            command.Name, 
            terms 
        );

        await repository.AddAsync(license);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully added a new license with Name: {0} and Id: {1}.", command.Name, license.Id);
        
        return license.Id;
    }
}