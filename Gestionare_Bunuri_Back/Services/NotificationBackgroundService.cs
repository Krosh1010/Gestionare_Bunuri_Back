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
    /// Background service care rulează periodic (la fiecare 15 minute),
    /// generează notificări pentru toți utilizatorii și trimite push notifications
    /// pe dispozitivele mobile, chiar dacă aplicația este închisă sau pe background.
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

            _logger.LogInformation("Începem procesarea notificărilor background...");

            // 1. Obținem toți utilizatorii care au dispozitive înregistrate
            var userIdsWithTokens = await deviceTokenRepository.GetAllUserIdsWithTokensAsync();

            if (!userIdsWithTokens.Any())
            {
                _logger.LogInformation("Nu există utilizatori cu device tokens înregistrate.");
                return;
            }

            // 2. Includem și utilizatorii care sunt proprietari de spații (pot avea notificări)
            var allOwnerUserIds = await notificationRepository.GetAllOwnerUserIdsAsync();

            // Luăm doar utilizatorii care au ȘI device tokens ȘI sunt proprietari
            var userIdsToProcess = userIdsWithTokens.Intersect(allOwnerUserIds).ToList();

            _logger.LogInformation("Procesăm notificări pentru {Count} utilizatori.", userIdsToProcess.Count);

            foreach (var userId in userIdsToProcess)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    await ProcessUserNotificationsAsync(
                        userId,
                        notificationRepository,
                        deviceTokenRepository,
                        pushNotificationService);
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
            IPushNotificationService pushNotificationService)
        {
            // 1. Generăm notificările (aceeași logică ca la GET /api/notifications)
            await notificationRepository.GenerateExpiringNotificationsAsync(userId);

            // 2. Obținem notificările care nu au fost trimise ca push
            var unsentNotifications = await notificationRepository.GetUnsentPushNotificationsAsync(userId);

            if (!unsentNotifications.Any())
            {
                return;
            }

            // 3. Obținem device tokens pentru acest user
            var deviceTokens = await deviceTokenRepository.GetTokensByUserIdAsync(userId);

            if (!deviceTokens.Any())
            {
                return;
            }

            _logger.LogInformation(
                "Trimitem {NotifCount} notificări push către {TokenCount} dispozitive pentru userId: {UserId}",
                unsentNotifications.Count, deviceTokens.Count, userId);

            var sentNotificationIds = new List<int>();

            foreach (var notification in unsentNotifications)
            {
                string title = notification.Type == NotificationType.WARRANTY_EXP
                    ? "Garanție"
                    : "Asigurare";

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
                        // Tokenul este invalid/expirat, îl ștergem
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

            // 4. Marcăm notificările ca trimise (push sent)
            if (sentNotificationIds.Any())
            {
                await notificationRepository.MarkNotificationsAsPushSentAsync(sentNotificationIds);
                _logger.LogInformation(
                    "Marcate {Count} notificări ca push-sent pentru userId: {UserId}",
                    sentNotificationIds.Count, userId);
            }
        }
    }
}
