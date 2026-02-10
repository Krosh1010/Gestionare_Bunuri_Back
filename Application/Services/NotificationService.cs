
using Domain.DbTables;
using Domain.Notification;
using Infrastructure.Abstraction;


namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task GenerateExpiringNotificationsAsync(int userId)
        {
            await _notificationRepository.GenerateExpiringNotificationsAsync(userId);
        }

        public async Task<List<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _notificationRepository.GetNotificationsByUserIdAsync(userId);
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            return await _notificationRepository.DeleteNotificationAsync(notificationId, userId);
        }
    }
}

