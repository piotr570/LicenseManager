using LicenseManager.Licenses.Application.DTOs;
using LicenseManager.Licenses.Application.Queries;
using LicenseManager.Licenses.Domain.Repositories;
using MediatR;

namespace LicenseManager.Licenses.Application.Handlers;

public sealed class GetAllLicensesQueryHandler(ILicenseRepository licenseRepository)
    : IRequestHandler<GetAllLicensesQuery, IEnumerable<LicenseDto>>
{
    public async Task<IEnumerable<LicenseDto>> Handle(GetAllLicensesQuery request, CancellationToken cancellationToken)
    {
        var licenses = await licenseRepository.GetAllAsync(cancellationToken);
        return licenses.Select(MapToDto).ToList();
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