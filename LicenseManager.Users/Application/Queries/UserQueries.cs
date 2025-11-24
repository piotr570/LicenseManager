using MediatR;

namespace LicenseManager.Users.Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;

public record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

