namespace LicenseManager.Notification.Application;

public class NotificationService : INotificationService
{
    private readonly List<Domain.Notification> _notifications = [];
    
    public void SendNotification(string message)
    {
        _notifications.Add(new Domain.Notification(message));
    }
    
    public IEnumerable<Domain.Notification> GetAllNotifications() => _notifications;
}