using LicenseManager.Notification.Application;
using Microsoft.AspNetCore.Mvc;

namespace LicenseManager.Notification.API;

[ApiController]
[Route("api/[controller]")]
public class NotificationController(INotificationService notificationService)
    : ControllerBase
{
    [HttpPost]
    public IActionResult Send([FromBody] string message)
    {
        notificationService.SendNotification(message);
        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(notificationService.GetAllNotifications());
    }
}