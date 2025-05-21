using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Commands;

public record ReserveLicenseCommand(Guid LicenseId, Guid UserId) : IRequest;
