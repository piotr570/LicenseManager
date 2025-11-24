namespace LicenseManager.Application.UseCases.Licenses.Dtos;

public record LicenseDto(
    Guid Id,
    string Key,
    string Name,
    string Vendor,
    bool IsActive,
    bool IsCancelled,
    LicenseTermsDto Terms,
    IReadOnlyCollection<Guid> Assignments,
    bool IsExpiringSoon = false);