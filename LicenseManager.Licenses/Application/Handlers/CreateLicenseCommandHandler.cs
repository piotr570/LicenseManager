using LicenseManager.Licenses.Application.Commands;
using LicenseManager.Licenses.Domain.Aggregates;
using LicenseManager.Licenses.Domain.Enums;
using LicenseManager.Licenses.Domain.Repositories;
using LicenseManager.Licenses.Domain.ValueObjects;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class CreateLicenseCommandHandler(ILicenseRepository licenseRepository)
    : IRequestHandler<CreateLicenseCommand, Guid>
{
    public async Task<Guid> Handle(CreateLicenseCommand request, CancellationToken cancellationToken)
    {
        var existingLicense = await licenseRepository.GetByKeyAsync(request.Key, cancellationToken);
        if (existingLicense != null)
            throw new InvalidOperationException($"License with key '{request.Key}' already exists.");

        var terms = new LicenseTerms(
            Enum.Parse<LicenseType>(request.Terms.Type),
            Enum.Parse<LicenseMode>(request.Terms.Mode),
            request.Terms.MaxUsers,
            request.Terms.IsRenewable,
            request.Terms.ExpirationDate,
            request.Terms.RenewalDate,
            request.Terms.UsageLimit);

        var license = new License(
            request.Key,
            request.Vendor,
            request.Name,
            terms,
            request.DepartmentId);

        await licenseRepository.AddAsync(license, cancellationToken);
        return license.Id;
    }
}