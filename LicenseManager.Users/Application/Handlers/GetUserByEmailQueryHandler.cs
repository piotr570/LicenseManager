using MediatR;
using LicenseManager.Users.Application.Queries;
using LicenseManager.Users.Domain.Repositories;

namespace LicenseManager.Users.Application.Handlers;

public sealed class GetUserByEmailQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(Domain.Aggregates.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            DepartmentId = user.DepartmentId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

