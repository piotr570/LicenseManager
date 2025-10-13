using LicenseManager.Application.UseCases.Users.Models;
using LicenseManager.Application.UseCases.Users.Queries;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class GetAllUsersQueryHandler(IRepository<User> repository,
    ILogger<GetAllUsersQueryHandler> logger)
    : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting all users.");
        var users = await repository.GetAllAsync(cancellationToken);
        var mappedUsers = users.Select(user => new UserDto(user));
        logger.LogInformation($"Returning fetched users.");
        return mappedUsers;
    }
}