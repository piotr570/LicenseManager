using LicenseManager.Licenses.Application.Commands;
using LicenseManager.Licenses.Application.DTOs;
using LicenseManager.Licenses.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LicenseManager.Licenses.API;

internal static class LicensesEndpoints
{
    internal static IEndpointRouteBuilder MapLicensesModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/license")
            .WithTags("Licenses Module");

        group.MapGet("/{licenseId:guid}", GetLicenseById)
            .WithName("GetLicenseById")
            .Produces<LicenseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        group.MapGet("/", GetAllLicenses)
            .WithName("GetAllLicenses")
            .Produces<IEnumerable<LicenseDto>>(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapPost("/", CreateLicense)
            .WithName("CreateLicense")
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithOpenApi();

        group.MapPost("/{licenseId:guid}/assign/{userId:guid}", AssignLicense)
            .WithName("AssignLicense")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapPost("/{licenseId:guid}/reserve/{userId:guid}", ReserveLicense)
            .WithName("ReserveLicense")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapPost("/{licenseId:guid}/invoke/{userId:guid}", InvokeLicense)
            .WithName("InvokeLicense")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapDelete("/{licenseId:guid}", DeleteLicense)
            .WithName("DeleteLicense")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapGet("/{licenseId:guid}/assignments", GetLicenseAssignments)
            .WithName("GetLicenseAssignments")
            .Produces<IEnumerable<AssignmentDto>>(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapGet("/user/{userId:guid}/assignments", GetUserAssignments)
            .WithName("GetUserAssignments")
            .Produces<IEnumerable<AssignmentDto>>(StatusCodes.Status200OK)
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> GetLicenseById(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId)
    {
        var result = await mediator.Send(new GetLicenseByIdQuery(licenseId));
        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> GetAllLicenses(
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetAllLicensesQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateLicense(
        [FromServices] IMediator mediator,
        [FromBody] CreateLicenseDto request)
    {
        var command = new CreateLicenseCommand(
            request.Key,
            request.Name,
            request.Vendor,
            request.Terms,
            request.DepartmentId);

        var licenseId = await mediator.Send(command);
        return Results.Created($"/license/{licenseId}", licenseId);
    }

    private static async Task<IResult> AssignLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId,
        [FromRoute] Guid userId)
    {
        await mediator.Send(new AssignLicenseCommand(licenseId, userId));
        return Results.NoContent();
    }

    private static async Task<IResult> ReserveLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId,
        [FromRoute] Guid userId)
    {
        await mediator.Send(new ReserveLicenseCommand(licenseId, userId));
        return Results.NoContent();
    }

    private static async Task<IResult> InvokeLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId,
        [FromRoute] Guid userId)
    {
        await mediator.Send(new InvokeLicenseCommand(licenseId, userId));
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteLicense(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId)
    {
        await mediator.Send(new DeleteLicenseCommand(licenseId));
        return Results.NoContent();
    }

    private static async Task<IResult> GetLicenseAssignments(
        [FromServices] IMediator mediator,
        [FromRoute] Guid licenseId)
    {
        var result = await mediator.Send(new GetLicenseAssignmentsQuery(licenseId));
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserAssignments(
        [FromServices] IMediator mediator,
        [FromRoute] Guid userId)
    {
        var result = await mediator.Send(new GetUserAssignmentsQuery(userId));
        return Results.Ok(result);
    }
}

public static class LicensesModuleEndpointExtensions
{
    public static IEndpointRouteBuilder MapLicensesModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapLicensesModuleEndpoints();
        return endpoints;
    }
}

