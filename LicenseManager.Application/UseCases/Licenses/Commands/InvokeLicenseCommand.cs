using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Commands;

public record InvokeLicenseCommand(Guid UserId, Guid LicenseId) : IRequest;
