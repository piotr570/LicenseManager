using LicenseManager.Licenses.Application.DTOs;
using LicenseManager.Licenses.Application.Queries;
using LicenseManager.Licenses.Domain.Repositories;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class GetLicenseAssignmentsQueryHandler(IAssignmentRepository assignmentRepository)
    : IRequestHandler<GetLicenseAssignmentsQuery, IEnumerable<AssignmentDto>>
{
    public async Task<IEnumerable<AssignmentDto>> Handle(GetLicenseAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await assignmentRepository.GetByLicenseIdAsync(request.LicenseId, cancellationToken);
        return assignments.Select(MapToDto).ToList();
    }

    private static AssignmentDto MapToDto(Domain.Aggregates.Assignment assignment)
    {
        return new AssignmentDto(
            assignment.Id,
            assignment.LicenseId,
            assignment.UserId,
            assignment.AssignedAt,
            assignment.LastInvokedAt,
            assignment.State.ToString());
    }
}