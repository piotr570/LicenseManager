using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Commands;

public record AssignLicenseCommand(Guid LicenseId, Guid UserId) : IRequest;