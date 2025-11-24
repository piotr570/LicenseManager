using LicenseManager.Users.Application.Queries;
using LicenseManager.Users.Domain.Repositories;
using MediatR;

namespace LicenseManager.Users.Application.Handlers;

public sealed class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto).ToList();
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