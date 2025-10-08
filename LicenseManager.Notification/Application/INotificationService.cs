namespace LicenseManager.Notification.Application;

public interface INotificationService
{
    void SendNotification(string message);
    IEnumerable<Domain.Notification> GetAllNotifications();
}