using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Commands;

public record DeleteLicenseCommand(Guid LicenseId) : IRequest;
