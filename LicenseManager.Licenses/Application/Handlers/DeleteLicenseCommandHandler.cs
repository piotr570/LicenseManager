using MediatR;
using LicenseManager.Licenses.Application.Commands;
using LicenseManager.Licenses.Domain.Repositories;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class DeleteLicenseCommandHandler(ILicenseRepository licenseRepository)
    : IRequestHandler<DeleteLicenseCommand>
{
    public async Task Handle(DeleteLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);
        if (license == null)
            throw new InvalidOperationException($"License with ID '{request.LicenseId}' not found.");

        await licenseRepository.DeleteAsync(request.LicenseId, cancellationToken);
    }
}

