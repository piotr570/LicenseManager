using MediatR;
using LicenseManager.Licenses.Application.DTOs;
using LicenseManager.Licenses.Application.Queries;
using LicenseManager.Licenses.Domain.Repositories;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class GetUserAssignmentsQueryHandler(IAssignmentRepository assignmentRepository)
    : IRequestHandler<GetUserAssignmentsQuery, IEnumerable<AssignmentDto>>
{
    public async Task<IEnumerable<AssignmentDto>> Handle(GetUserAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
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

