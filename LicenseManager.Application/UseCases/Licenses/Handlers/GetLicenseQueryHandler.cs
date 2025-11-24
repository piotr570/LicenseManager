using LicenseManager.Application.UseCases.Licenses.Dtos;
using LicenseManager.Application.UseCases.Licenses.Queries;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Assignments;
using LicenseManager.SharedKernel.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Application.UseCases.Licenses.Handlers;

public class GetLicenseQueryHandler(IReadDbContext db) : IRequestHandler<GetLicenseByIdQuery, LicenseDto>
{
    public async Task<LicenseDto> Handle(GetLicenseByIdQuery query, CancellationToken cancellationToken)
    {
        var licenseId = query.LicenseId;

        var license = await db.Set<License>()
            .Include(x => x.Assignments)
            .Where(x => x.Id == licenseId)
            .Select(x => new
            {
                x.Id,
                x.Key,
                x.Name,
                x.Vendor,
                x.IsActive,
                x.IsCancelled,
                Terms_Type = x.Terms.Type,
                Terms_Mode = x.Terms.Mode,
                Terms_MaxUsers = x.Terms.MaxUsers,
                Terms_UsageLimit = x.Terms.UsageLimit,
                Terms_ExpirationDate = x.Terms.ExpirationDate,
                Terms_IsRenewable = x.Terms.IsRenewable,
                Terms_RenewalDate = x.Terms.RenewalDate,
                XXXAssignment = x.Assignments.Select(z => z.UserId)
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (license == null)
            throw new NotFoundException(nameof(License));

        // var assignments = (await db.Set<Assignment>()
        //     .Where(x => x.LicenseId == licenseId)
        //     .Select(x => x.UserId)
        //     .Distinct()
        //     .ToListAsync(cancellationToken))
        //     .AsReadOnly();

        var termsDto = new LicenseTermsDto(
            license.Terms_Type,
            license.Terms_Mode,
            license.Terms_MaxUsers,
            license.Terms_UsageLimit,
            license.Terms_ExpirationDate,
            license.Terms_IsRenewable,
            license.Terms_RenewalDate
        );

        return new LicenseDto(
            license.Id,
            license.Key,
            license.Name,
            license.Vendor,
            license.IsActive,
            license.IsCancelled,
            termsDto,
            license.XXXAssignment.ToList()
        );
    }
}