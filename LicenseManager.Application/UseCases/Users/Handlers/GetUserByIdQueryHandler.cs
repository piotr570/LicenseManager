using LicenseManager.Application.UseCases.Users.Models;
using LicenseManager.Application.UseCases.Users.Queries;
using LicenseManager.Domain.Users;
using LicenseManager.SharedKernel.Abstractions;
using LicenseManager.SharedKernel.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class GetUserByIdQueryHandler(IReadDbContext db,
    ILogger<GetUserByIdQueryHandler> logger) 
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user with Id: {0}", query.UserId);
        var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == query.UserId, cancellationToken)
                   ?? throw new NotFoundException(nameof(User));
        var mappedUser = new UserDto(user);
        logger.LogInformation("Returning user with Id: {0}", query.UserId);
        return mappedUser;
    }
}