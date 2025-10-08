using LicenseManager.Notification.Application;
using Microsoft.Extensions.DependencyInjection;

namespace LicenseManager.Notification.Infrastructure;

public static class NotificationModuleRegistration
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services)
    {
        services.AddSingleton<INotificationService, NotificationService>();
        return services;
    }
}