using Domain.DbTables;

namespace Application.Abstraction
{
    public interface IPushNotificationService
    {
        Task SendPushNotificationAsync(string deviceToken, string title, string body);
        Task SendPushNotificationsAsync(List<string> deviceTokens, string title, string body);
    }
}
