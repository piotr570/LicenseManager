using LicenseManager.Application.UseCases.Audit.Models;
using MediatR;

namespace LicenseManager.Application.UseCases.Audit.Commands;

public record GenerateAuditReportCommand(string EmailAddress, 
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<AuditReportDto>;