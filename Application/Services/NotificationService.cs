
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

        public async Task<List<int>> GetAllOwnerUserIdsAsync()
        {
            return await _notificationRepository.GetAllOwnerUserIdsAsync();
        }

        public async Task<List<NotificationTable>> GetUnsentPushNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUnsentPushNotificationsAsync(userId);
        }

        public async Task MarkNotificationsAsPushSentAsync(List<int> notificationIds)
        {
            await _notificationRepository.MarkNotificationsAsPushSentAsync(notificationIds);
        }

        public async Task<List<NotificationTable>> GetUnsentEmailNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUnsentEmailNotificationsAsync(userId);
        }

        public async Task MarkNotificationsAsEmailSentAsync(List<int> notificationIds)
        {
            await _notificationRepository.MarkNotificationsAsEmailSentAsync(notificationIds);
        }
    }
}

