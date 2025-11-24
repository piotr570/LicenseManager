using LicenseManager.Application.UseCases.Licenses.Dtos;
using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Queries;

public record GetLicenseByIdQuery(Guid LicenseId) : IRequest<LicenseDto>;
