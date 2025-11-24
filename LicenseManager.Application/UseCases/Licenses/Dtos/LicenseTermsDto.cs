using LicenseManager.Domain.Licenses.Enums;

namespace LicenseManager.Application.UseCases.Licenses.Dtos;

public record LicenseTermsDto(
    LicenseType Type,
    LicenseMode Mode,
    int? MaxUsers,
    int? UsageLimit,
    DateTime? ExpirationDate,
    bool IsRenewable,
    DateTime? RenewalDate);