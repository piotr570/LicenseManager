using LicenseManager.Notification.API;
using LicenseManager.Notification.Application;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Notification.Infrastructure;

public static class NotificationModuleRegistration
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services,
        IMvcBuilder? mvcBuilder = null)
    {
        services.AddSingleton<INotificationService, NotificationService>();
        mvcBuilder?.AddApplicationPart(typeof(NotificationController).Assembly);
        return services;
    }
}