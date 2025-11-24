using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace LicenseManager.Users.Application;

public static class UsersApplicationRegistration
{
    private static readonly Assembly ApplicationAssembly = typeof(UsersApplicationRegistration).Assembly;

    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(ApplicationAssembly));
        return services;
    }
}

