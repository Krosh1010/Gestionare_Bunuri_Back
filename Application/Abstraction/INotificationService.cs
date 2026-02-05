using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DbTables;

public interface INotificationService
{
    Task GenerateExpiringNotificationsAsync(int userId);
    Task<List<NotificationTable>> GetNotificationsByUserIdAsync(int userId);
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);
}
