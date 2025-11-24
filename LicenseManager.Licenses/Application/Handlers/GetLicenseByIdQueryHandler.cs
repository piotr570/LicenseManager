using LicenseManager.Licenses.Application.DTOs;
using LicenseManager.Licenses.Application.Queries;
using LicenseManager.Licenses.Domain.Repositories;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class GetLicenseByIdQueryHandler(ILicenseRepository licenseRepository)
    : IRequestHandler<GetLicenseByIdQuery, LicenseDto?>
{
    public async Task<LicenseDto?> Handle(GetLicenseByIdQuery request, CancellationToken cancellationToken)
    {
        var license = await licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);
        return license == null ? null : MapToDto(license);
    }

    private static LicenseDto MapToDto(Domain.Aggregates.License license)
    {
        var termsDto = new LicenseTermsDto(
            license.Terms.Type.ToString(),
            license.Terms.Mode.ToString(),
            license.Terms.MaxUsers,
            license.Terms.IsRenewable,
            license.Terms.ExpirationDate,
            license.Terms.RenewalDate,
            license.Terms.UsageLimit);

        return new LicenseDto(
            license.Id,
            license.Key,
            license.Name,
            license.Vendor,
            termsDto,
            license.IsActive,
            license.UsageCount,
            license.IsPaymentValid,
            license.IsCancelled,
            license.DepartmentId);
    }
}