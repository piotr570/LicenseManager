using LicenseManager.Application.UseCases.Licenses.Commands;
using LicenseManager.Application.UseCases.Users.Commands;
using LicenseManager.Application.UseCases.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManager.Controllers;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/", GetAllUsers)
            .WithName("GetAllUsers")
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .Produces(StatusCodes.Status201Created)
            .WithOpenApi();

        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapPost("/{userId:guid}/licenses/{licenseId:guid}/invoke", InvokeLicense)
            .WithName("InvokeLicense")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();
    }

    private static async Task<IResult> GetAllUsers([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetAllUsersQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserById(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id));
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserCommand command,
        [FromServices] IMediator mediator)
    {
        var id = await mediator.Send(command);
        return Results.Created($"/users/{id}", id);
    }

    private static async Task<IResult> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UpdateUserCommand command,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUser(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(new DeleteUserCommand(id));
        return Results.NoContent();
    }

    private static async Task<IResult> InvokeLicense(
        [FromRoute] Guid userId,
        [FromRoute] Guid licenseId,
        [FromServices] IMediator mediator)
    {
        await mediator.Send(new InvokeLicenseCommand(userId, licenseId));
        return Results.NoContent();
    }
}