using MediatR;
using LicenseManager.Licenses.Application.DTOs;

namespace LicenseManager.Licenses.Application.Queries;

public record GetLicenseByIdQuery(Guid LicenseId) : IRequest<LicenseDto?>;

public record GetAllLicensesQuery : IRequest<IEnumerable<LicenseDto>>;

public record GetLicenseByKeyQuery(string Key) : IRequest<LicenseDto?>;

public record GetLicenseAssignmentsQuery(Guid LicenseId) : IRequest<IEnumerable<AssignmentDto>>;

public record GetUserAssignmentsQuery(Guid UserId) : IRequest<IEnumerable<AssignmentDto>>;

