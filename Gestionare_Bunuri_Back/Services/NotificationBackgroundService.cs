using Application.Abstraction;
using Domain.DbTables;
using FirebaseAdmin.Messaging;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gestionare_Bunuri_Back.Services
{
    /// <summary>
    /// Background service care rulează periodic (la fiecare 2 minute),
    /// generează notificări pentru toți utilizatorii și trimite push notifications
    /// pe dispozitivele mobile și email-uri, chiar dacă aplicația este închisă sau pe background.
    /// </summary>
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;

        // Interval de verificare: la fiecare 2 minute (pentru testare)
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2);

        public NotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NotificationBackgroundService pornit. Verificare la fiecare {Interval} minute.",
                _checkInterval.TotalMinutes);

            // Așteptăm puțin la pornire pentru a lăsa aplicația să se inițializeze complet
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNotificationsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Eroare în NotificationBackgroundService");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
        {
            // Creăm un scope nou pentru DI (serviciile sunt Scoped, nu Singleton)
            using var scope = _serviceProvider.CreateScope();

            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var deviceTokenRepository = scope.ServiceProvider.GetRequiredService<IDeviceTokenRepository>();
            var pushNotificationService = scope.ServiceProvider.GetRequiredService<IPushNotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            _logger.LogInformation("Începem procesarea notificărilor background...");

            // 1. Obținem toți utilizatorii care sunt proprietari de spații (pot avea notificări)
            var allOwnerUserIds = await notificationRepository.GetAllOwnerUserIdsAsync();

            if (!allOwnerUserIds.Any())
            {
                _logger.LogInformation("Nu există utilizatori proprietari de spații.");
                return;
            }

            // 2. Obținem utilizatorii cu device tokens pentru push
            var userIdsWithTokens = await deviceTokenRepository.GetAllUserIdsWithTokensAsync();

            _logger.LogInformation("Procesăm notificări pentru {Count} utilizatori proprietari.", allOwnerUserIds.Count);

            foreach (var userId in allOwnerUserIds)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    await ProcessUserNotificationsAsync(
                        userId,
                        notificationRepository,
                        deviceTokenRepository,
                        pushNotificationService,
                        emailService,
                        userRepository,
                        userIdsWithTokens.Contains(userId));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Eroare la procesarea notificărilor pentru userId: {UserId}", userId);
                }
            }

            _logger.LogInformation("Procesarea notificărilor background finalizată.");
        }

        private async Task ProcessUserNotificationsAsync(
            int userId,
            INotificationRepository notificationRepository,
            IDeviceTokenRepository deviceTokenRepository,
            IPushNotificationService pushNotificationService,
            IEmailService emailService,
            IUserRepository userRepository,
            bool hasDeviceTokens)
        {
            // 1. Generăm notificările (aceeași logică ca la GET /api/notifications)
            await notificationRepository.GenerateExpiringNotificationsAsync(userId);

            // 2. Trimitem push notifications (doar dacă are device tokens)
            if (hasDeviceTokens)
            {
                await SendPushNotificationsForUser(userId, notificationRepository, deviceTokenRepository, pushNotificationService);
            }

            // 3. Trimitem email notifications
            await SendEmailNotificationsForUser(userId, notificationRepository, emailService, userRepository);
        }

        private async Task SendPushNotificationsForUser(
            int userId,
            INotificationRepository notificationRepository,
            IDeviceTokenRepository deviceTokenRepository,
            IPushNotificationService pushNotificationService)
        {
            var unsentNotifications = await notificationRepository.GetUnsentPushNotificationsAsync(userId);

            if (!unsentNotifications.Any()) return;

            var deviceTokens = await deviceTokenRepository.GetTokensByUserIdAsync(userId);

            if (!deviceTokens.Any()) return;

            _logger.LogInformation(
                "Trimitem {NotifCount} notificări push către {TokenCount} dispozitive pentru userId: {UserId}",
                unsentNotifications.Count, deviceTokens.Count, userId);

            var sentNotificationIds = new List<int>();

            foreach (var notification in unsentNotifications)
            {
                string title = GetNotificationTitle(notification);
                string body = notification.Message;

                foreach (var deviceToken in deviceTokens)
                {
                    try
                    {
                        await pushNotificationService.SendPushNotificationAsync(
                            deviceToken.Token, title, body);
                    }
                    catch (FirebaseMessagingException ex) when (
                        ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                        ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                    {
                        _logger.LogWarning("Ștergem token invalid: {Token}", deviceToken.Token);
                        await deviceTokenRepository.RemoveInvalidTokenAsync(deviceToken.Token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Eroare la trimiterea push pentru notificarea {NotifId} către token {Token}",
                            notification.Id, deviceToken.Token);
                    }
                }

                sentNotificationIds.Add(notification.Id);
            }

            if (sentNotificationIds.Any())
            {
                await notificationRepository.MarkNotificationsAsPushSentAsync(sentNotificationIds);
                _logger.LogInformation(
                    "Marcate {Count} notificări ca push-sent pentru userId: {UserId}",
                    sentNotificationIds.Count, userId);
            }
        }

        private async Task SendEmailNotificationsForUser(
            int userId,
            INotificationRepository notificationRepository,
            IEmailService emailService,
            IUserRepository userRepository)
        {
            var unsentEmailNotifications = await notificationRepository.GetUnsentEmailNotificationsAsync(userId);

            if (!unsentEmailNotifications.Any()) return;

            // Obținem datele utilizatorului (email + nume)
            var userInfo = await userRepository.GetUserByIdAsync(userId);
            if (userInfo == null) return;

            _logger.LogInformation(
                "Trimitem {Count} notificări pe email către {Email} pentru userId: {UserId}",
                unsentEmailNotifications.Count, userInfo.Email, userId);

            try
            {
                // Construim lista de items pentru email
                var emailItems = unsentEmailNotifications.Select(n => new NotificationEmailItem
                {
                    Title = GetNotificationTitle(n),
                    Message = n.Message,
                    IsExpired = n.IsExpired
                }).ToList();

                // Trimitem un singur email cu toate notificările
                await emailService.SendNotificationEmailAsync(userInfo.Email, userInfo.FullName, emailItems);

                // Marcăm toate notificările ca trimise pe email
                var sentIds = unsentEmailNotifications.Select(n => n.Id).ToList();
                await notificationRepository.MarkNotificationsAsEmailSentAsync(sentIds);

                _logger.LogInformation(
                    "Marcate {Count} notificări ca email-sent pentru userId: {UserId}",
                    sentIds.Count, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Eroare la trimiterea email-ului cu notificări pentru userId: {UserId}", userId);
            }
        }

        private static string GetNotificationTitle(NotificationTable notification)
        {
            return notification.Type switch
            {
                NotificationType.WARRANTY_EXP => "Garanție",
                NotificationType.INSURANCE_EXP => "Asigurare",
                NotificationType.CUSTOM_TRACKER_EXP => notification.CustomTracker?.Name ?? "Tracker",
                _ => "Notificare"
            };
        }
    }
}
