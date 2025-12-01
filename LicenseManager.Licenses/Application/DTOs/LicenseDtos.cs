namespace LicenseManager.Licenses.Application.DTOs;

public record LicenseTermsDto(
    string Type,
    string Mode,
    int? MaxUsers,
    bool IsRenewable,
    DateTime? ExpirationDate,
    DateTime? RenewalDate,
    int? UsageLimit);

public record CreateLicenseDto(
    string Key,
    string Name,
    string Vendor,
    LicenseTermsDto Terms,
    Guid? DepartmentId);

public record LicenseDto(
    Guid Id,
    string Key,
    string Name,
    string Vendor,
    LicenseTermsDto Terms,
    bool IsActive,
    int UsageCount,
    bool? IsPaymentValid,
    bool IsCancelled,
    Guid? DepartmentId);

public record AssignmentDto(
    Guid Id,
    Guid LicenseId,
    Guid UserId,
    DateTime AssignedAt,
    DateTime? LastInvokedAt,
    string State);