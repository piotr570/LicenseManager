using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using LicenseManager.Users.Domain.Repositories;
using LicenseManager.Users.Infrastructure.Persistence;

namespace LicenseManager.Users.Infrastructure;

public static class UsersInfrastructureRegistration
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}