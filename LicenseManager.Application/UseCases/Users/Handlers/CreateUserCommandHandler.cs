using LicenseManager.Application.Abstraction;
using LicenseManager.Application.UseCases.Users.Commands;
using LicenseManager.Domain.Abstractions;
using LicenseManager.Domain.Common.Exceptions;
using LicenseManager.Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LicenseManager.Application.UseCases.Users.Handlers;

public class CreateUserCommandHandler(IRepository<User> repository, 
    IUnitOfWork unitOfWork,
    ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating a user with an email: {0}", command.Email);

        var user = new User(command.Email, command.Name, command.Department);
        
        await repository.AddAsync(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully created a user with an email: {0} and corresponding id: {1}", command.Email, user.Id);
        return user.Id;
    }
}