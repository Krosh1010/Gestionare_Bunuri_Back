using Domain.Notification;

public interface INotificationService
{
    Task GenerateExpiringNotificationsAsync(int userId);
    Task<List<NotificationDto>> GetNotificationsByUserIdAsync(int userId);
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
}
