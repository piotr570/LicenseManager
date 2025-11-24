using LicenseManager.Users.Application.Commands;
using LicenseManager.Users.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LicenseManager.Users.API;

internal static class UsersEndpoints
{
    internal static IEndpointRouteBuilder MapUsersModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/users")
            .WithTags("Users Module");

        group.MapGet("/", GetAllUsers)
            .WithName("GetAllUsers")
            .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapGet("/{userId:guid}", GetUserById)
            .WithName("GetUserById")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithOpenApi();

        group.MapPut("/{userId:guid}", UpdateUser)
            .WithName("UpdateUser")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapPost("/{userId:guid}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapPost("/{userId:guid}/activate", ActivateUser)
            .WithName("ActivateUser")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        group.MapDelete("/{userId:guid}", DeleteUser)
            .WithName("DeleteUser")
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> GetAllUsers(
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetAllUsersQuery());
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserById(
        [FromServices] IMediator mediator,
        [FromRoute] Guid userId)
    {
        var result = await mediator.Send(new GetUserByIdQuery(userId));
        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> CreateUser(
        [FromServices] IMediator mediator,
        [FromBody] CreateUserCommand command)
    {
        var userId = await mediator.Send(command);
        return Results.Created($"/users/{userId}", userId);
    }

    private static async Task<IResult> UpdateUser(
        [FromServices] IMediator mediator,
        [FromRoute] Guid userId,
        [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand(userId, request.Email, request.Name, request.DepartmentId);
        await mediator.Send(command);
        return Results.NoContent();
    }

    private static async Task<IResult> DeactivateUser(
        [FromServices] IMediator mediator,
        [FromRoute] Guid userId)
    {
        await mediator.Send(new DeactivateUserCommand(userId));
        return Results.NoContent();
    }

    private static async Task<IResult> ActivateUser(
        [FromServices] IMediator mediator,
        [FromRoute] Guid userId)
    {
        await mediator.Send(new ActivateUserCommand(userId));
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUser(
        [FromServices] IMediator mediator,
        [FromRoute] Guid userId)
    {
        await mediator.Send(new DeleteUserCommand(userId));
        return Results.NoContent();
    }
}

public record UpdateUserRequest(string Email, string Name, Guid? DepartmentId);

public record UserDto(
    Guid Id,
    string Email,
    string Name,
    Guid? DepartmentId,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public static class UsersModuleEndpointExtensions
{
    public static IEndpointRouteBuilder MapUsersModule(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapUsersModuleEndpoints();
        return endpoints;
    }
}

