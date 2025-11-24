using LicenseManager.Licenses.Application.Commands;
using LicenseManager.Licenses.Domain.Repositories;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class ReserveLicenseCommandHandler(ILicenseRepository licenseRepository)
    : IRequestHandler<ReserveLicenseCommand>
{
    public async Task Handle(ReserveLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);
        if (license == null)
            throw new InvalidOperationException($"License with ID '{request.LicenseId}' not found.");

        license.Reserve(request.UserId);
        await licenseRepository.UpdateAsync(license, cancellationToken);
    }
}