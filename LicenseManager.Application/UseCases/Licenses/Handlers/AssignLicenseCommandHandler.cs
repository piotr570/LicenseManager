using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Licenses.Abstractions;
using LicenseManager.Domain.Licenses.Factories.Assignment;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class AssignLicenseCommandHandler(
    ILicenseBusinessRuleProvider ruleProvider,
    ILicenseAssignmentPolicyFactory policyFactory,
    IRepository<License> licenseRepository,
    IRepository<User> userRepository,
    IUnitOfWork unitOfWork,
    ILogger<AssignLicenseCommandHandler> logger)
    : IRequestHandler<AssignLicenseCommand>
{
    public async Task Handle(AssignLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assigning a license with Id: {0} to Id: {1}.", command.LicenseId, command.UserId);

        var license = await licenseRepository.GetByIdIncludingAsync(command.LicenseId, 
                          x => x.Terms,
                          x => x.Reservations,
                          x => x.Assignments)
                      ?? throw new NotFoundException(nameof(License), command.LicenseId);

        var user = await userRepository.GetByIdIncludingAsync(command.UserId, 
                       x => x.LicenseAssignments,
                       x => x.LicenseReservations)
                   ?? throw new NotFoundException(nameof(User), command.UserId);
        
        var policy = policyFactory.GetPolicy(license.Terms.Type);
        var rules = ruleProvider.GetRules(license, user);
        
        license.AssignUser(user, policy, rules, SystemClock.Now);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully assigned a license with Id: {0} to Id: {1}.", command.LicenseId, command.UserId);
    }
}