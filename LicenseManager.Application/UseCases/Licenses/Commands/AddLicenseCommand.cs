using LicenseManager.Domain.Licenses.Enums;
using MediatR;

namespace LicenseManager.Application.UseCases.Licenses.Commands;

public sealed record AddLicenseCommand(
    string Key,
    string Vendor,
    string Name,
    LicenseType LicenseType,
    LicenseMode LicenseMode,
    bool IsRenewable,
    int? MaxUsers = null,
    int? UsageLimit = null,
    DateTime? RenewalDate = null,
    DateTime? ExpirationDate = null) : IRequest<Guid>;
