using LicenseManager.Licenses.Application.Commands;
using LicenseManager.Licenses.Domain.Aggregates;
using LicenseManager.Licenses.Domain.Policies;
using LicenseManager.Licenses.Domain.Repositories;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class AssignLicenseCommandHandler(
    ILicenseRepository licenseRepository,
    IAssignmentRepository assignmentRepository,
    ILicenseAssignmentPolicyFactory policyFactory)
    : IRequestHandler<AssignLicenseCommand>
{
    public async Task Handle(AssignLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);
        if (license == null)
            throw new InvalidOperationException($"License with ID '{request.LicenseId}' not found.");

        if (!license.IsActive)
            throw new InvalidOperationException("Cannot assign inactive license.");

        var existingAssignments = await assignmentRepository.GetByLicenseIdAsync(request.LicenseId, cancellationToken);
        
        var policy = policyFactory.GetPolicy(license.Terms.Type);
        policy.ValidateAssignment(
            existingAssignments.Count(),
            license.Terms.MaxUsers,
            license.DepartmentId,
            null); // In real scenario, would need user department

        var assignment = new Assignment(request.LicenseId, request.UserId, DateTime.UtcNow);
        await assignmentRepository.AddAsync(assignment, cancellationToken);

        license.AssignUser(assignment);
        await licenseRepository.UpdateAsync(license, cancellationToken);
    }
}