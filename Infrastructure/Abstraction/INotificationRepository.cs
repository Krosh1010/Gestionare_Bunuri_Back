using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DbTables;
using Domain.Notification;

namespace Infrastructure.Abstraction
{
    public interface INotificationRepository
    {
        Task GenerateExpiringNotificationsAsync(int userId);
        Task<List<NotificationDto>> GetNotificationsByUserIdAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
    }
}
