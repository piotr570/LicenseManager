using LicenseManager.Users.Application;
using LicenseManager.Users.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Users;

public static class UsersModuleRegistration
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddUsersApplication();
        services.AddUsersInfrastructure(connectionString);
        return services;
    }
}