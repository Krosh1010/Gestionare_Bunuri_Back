using Application.Abstraction;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace Gestionare_Bunuri_Back.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly ILogger<PushNotificationService> _logger;

        public PushNotificationService(ILogger<PushNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendPushNotificationAsync(string deviceToken, string title, string body)
        {
            try
            {
                var message = new Message
                {
                    Token = deviceToken,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    // Configurare pentru a afișa notificarea chiar dacă aplicația e pe background
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Sound = "default",
                            ChannelId = "asset_guard_notifications"
                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Sound = "default",
                            ContentAvailable = true
                        }
                    }
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation("Push notification trimisă cu succes: {Response}", response);
            }
            catch (FirebaseMessagingException ex) when (
                ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                _logger.LogWarning("Token invalid sau expirat: {Token}. Eroare: {Error}", deviceToken, ex.Message);
                throw; // Permite background service-ului să șteargă tokenul invalid
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eroare la trimiterea push notification către token: {Token}", deviceToken);
            }
        }

        public async Task SendPushNotificationsAsync(List<string> deviceTokens, string title, string body)
        {
            foreach (var token in deviceTokens)
            {
                await SendPushNotificationAsync(token, title, body);
            }
        }
    }
}
