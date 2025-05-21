using LicenseManager.Application.UseCases.Licenses.Models;
using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Queries;

public record GetLicenseByIdQuery(Guid LicenseId) : IRequest<LicenseDto>;
