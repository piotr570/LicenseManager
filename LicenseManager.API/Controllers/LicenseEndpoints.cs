using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Application.UseCases.Licenses.Models;
using LicenseManager.Application.UseCases.Licenses.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManager.Controllers;

public static class LicenseEndpoints
{
    public static WebApplication MapLicenseEndpoints(this WebApplication app)
    {
        var licenseGroup = app.MapGroup("/license");

        licenseGroup.MapGet("/{licenseId:guid}", GetLicenseById)
            .WithName("GetLicenseById")
            .Produces<LicenseDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        licenseGroup.MapPost("/", AddLicense)
            .WithName("AddLicense")
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithOpenApi();

        licenseGroup.MapPost("/{licenseId:guid}/assign/{userId:guid}", AssignLicense)
            .WithName("AssignLicense")
            .WithOpenApi();

        licenseGroup.MapDelete("/{licenseId:guid}", DeleteLicense)
            .WithName("DeleteLicense")
            .WithOpenApi();

        licenseGroup.MapPost("/{licenseId:guid}/reserve/{userId:guid}", ReserveLicense)
            .WithName("ReserveLicense")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> GetLicenseById(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId)
    {
        var query = new GetLicenseByIdQuery(licenseId);
        var result = await mediator.Send(query);
        return Results.Ok(result);
    }

    private static async Task<IResult> AddLicense(
        [FromServices] IMediator mediator,
        [FromBody] AddLicenseCommand command)
    {
        var licenseId = await mediator.Send(command);
        return Results.Created($"/license/{licenseId}", licenseId);
    }

    private static async Task<IResult> AssignLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId,
        [FromRoute] Guid userId)
    {
        var command = new AssignLicenseCommand(licenseId, userId);
        await mediator.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId)
    {
        await mediator.Send(new DeleteLicenseCommand(licenseId));
        return Results.NoContent();
    }

    private static async Task<IResult> ReserveLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId,
        [FromRoute] Guid userId)
    {
        var command = new ReserveLicenseCommand(licenseId, userId);
        await mediator.Send(command);
        return Results.NoContent();
    }
}