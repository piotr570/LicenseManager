using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Domain.Assignments;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Common;
using LicenseManager.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class AssignLicenseCommandHandler(
    ILicenseAssignmentPolicyFactory policyFactory,
    IReadDbContext db,
    IRepository<Assignment> repository,
    IUnitOfWork unitOfWork,
    ILogger<AssignLicenseCommandHandler> logger)
    : IRequestHandler<AssignLicenseCommand>
{
    public async Task Handle(AssignLicenseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Assigning a license with Id: {0} to Id: {1}.", command.LicenseId, command.UserId);

        // Load license aggregate including assignments and user
        var license = await db.Set<License>()
            .Include(x => x.Assignments)
            .FirstOrDefaultAsync(x => x.Id == command.LicenseId, cancellationToken);

        var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken);

        if (license == null || user == null)
            throw new NotFoundException("Either License or User was not found.");

        // Ensure assignment does not already exist (check the child collection)
        var assignmentExists = license.Assignments.Any(x => x.UserId == command.UserId);

        if (assignmentExists)
            throw new ConflictException("The License is already assigned to the User.");

        // Use policy to validate assignment according to license terms
        var policy = policyFactory.GetPolicy(license.Terms.Type);
        var currentAssignmentCount = license.Assignments.Count;
        policy.CanAssignUser(currentAssignmentCount, license.Terms.MaxUsers, license.Department, user.Department);

        // Update aggregate (this will create and add Assignment as child)
        var now = SystemClock.Now;
        var assignment = new Assignment(license.Id, user.Id, now);
        license.AssignUser(assignment);

        await repository.AddAsync(assignment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully assigned a license with Id: {0} to Id: {1}.", command.LicenseId, command.UserId);
    }
}