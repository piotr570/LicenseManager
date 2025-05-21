using LicenseManager.Application.UseCases.Audit.Commands;
using LicenseManager.Application.UseCases.Audit.Models;
using LicenseManager.Application.UseCases.Licenses.Models;
using LicenseManager.Application.UseCases.Users.Models;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common;
using LicenseManager.Domain.Licenses;
using LicenseManager.Domain.Users;
using MediatR;

namespace LicenseManager.Application.UseCases.Audit.Handlers;

public class GenerateAuditReportCommandHandler(
    IRepository<License> licenseRepository,
    IRepository<User> userRepository)
    : IRequestHandler<GenerateAuditReportCommand, AuditReportDto>
{
    public async Task<AuditReportDto> Handle(GenerateAuditReportCommand request, CancellationToken cancellationToken)
    {
        var licenses = await licenseRepository.GetAllAsync(cancellationToken);
        var users = await userRepository.GetAllAsync(cancellationToken);

        var activeLicenses = licenses.Where(l => l is { IsActive: true, IsCancelled: false }).ToList();
        var expiringSoon = activeLicenses
            .Where(l => l.Terms.ExpirationDate.HasValue && 
                        l.Terms.ExpirationDate.Value <= SystemClock.Now.AddDays(30))
            .ToList();

        return new AuditReportDto
        {
            ActiveLicenses = activeLicenses.Select(l => new LicenseDto(l)).ToList(),
            Users = users.Select(u => new UserDto(u)).ToList(),
            ExpiringLicenses = expiringSoon.Select(l => new LicenseDto(l, true)).ToList(),
            EmailAddress = request.EmailAddress,
            GeneratedAt = SystemClock.Now
        };
    }
}
