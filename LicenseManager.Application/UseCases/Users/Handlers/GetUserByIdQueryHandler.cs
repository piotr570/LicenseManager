using LicenseManager.Application.Common.Exceptions;
using LicenseManager.Application.UseCases.Users.Models;
using LicenseManager.Application.UseCases.Users.Queries;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class GetUserByIdQueryHandler(IRepository<User> repository,
    ILogger<GetUserByIdQueryHandler> logger) 
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user with Id: {0}", query.UserId);
        var user = await repository.GetByIdAsync(query.UserId, cancellationToken) 
                   ?? throw new NotFoundException(nameof(User), query.UserId);
        var mappedUser = new UserDto(user);
        logger.LogInformation("Returning user with Id: {0}", query.UserId);
        return mappedUser;
    }
}