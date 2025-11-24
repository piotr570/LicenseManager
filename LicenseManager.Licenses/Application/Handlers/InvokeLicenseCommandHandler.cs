using LicenseManager.Licenses.Application.Commands;
using LicenseManager.Licenses.Domain.Repositories;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class InvokeLicenseCommandHandler(ILicenseRepository licenseRepository)
    : IRequestHandler<InvokeLicenseCommand>
{
    public async Task Handle(InvokeLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);
        if (license == null)
            throw new InvalidOperationException($"License with ID '{request.LicenseId}' not found.");

        license.Invoke(request.UserId);
        await licenseRepository.UpdateAsync(license, cancellationToken);
    }
}