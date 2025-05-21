using LicenseManager.Application.UseCases.Audit.Commands;
using LicenseManager.Application.UseCases.Audit.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManager.Modules;

public static class AuditModule
{
    public static WebApplication AddAuditModule(this WebApplication app)
    {
        var auditGroup = app.MapGroup("/audit");

        auditGroup.MapGet("/", GenerateAuditReport)
            .WithName("GenerateAuditReport")
            .Produces<AuditReportDto>()
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GenerateAuditReport(
        [FromServices] IMediator mediator,
        [AsParameters] GenerateAuditReportCommand query)
    {
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }
}