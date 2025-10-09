using LicenseManager.Notification.API;
using LicenseManager.Notification.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace LicenseManager.Notification.Infrastructure;

public static class NotificationModuleRegistration
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services)
    {
        services.AddSingleton<INotificationService, NotificationService>();
        return services;
    }

    public static IEndpointRouteBuilder MapNotificationModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapNotificationEndpoints();
        return endpoints;
    }
}