using LicenseManager.Application.UseCases.Users.Models;
using MediatR;

namespace LicenseManager.Application.UseCases.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;