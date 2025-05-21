using LicenseManager.Application.UseCases.Licenses.Models;
using LicenseManager.Application.UseCases.Users.Models;

namespace LicenseManager.Application.UseCases.Audit.Models;

public class AuditReportDto
{
    public List<LicenseDto> ActiveLicenses { get; set; } = null!;
    public List<UserDto> Users { get; set; } = null!;
    public List<LicenseDto> ExpiringLicenses { get; set; } = null!;
    public DateTime GeneratedAt { get; set; }
    public string EmailAddress { get; set; } = null!;
}