using LicenseManager.Notification.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace LicenseManager.Notification.API;

internal static class NotificationEndpoints
{
    internal static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var notificationGroup = endpoints.MapGroup("/notification")
            .WithTags("NotificationModule");

        notificationGroup.MapPost("/",
            (INotificationService notificationService, [FromBody] string message) =>
        {
            if (string.IsNullOrWhiteSpace(message))
                return Results.BadRequest();
            
            notificationService.SendNotification(message);
            return Results.Ok();
        });

        notificationGroup.MapGet("/", (INotificationService notificationService) =>
            Results.Ok(notificationService.GetAllNotifications()));

        return endpoints;
    }
}