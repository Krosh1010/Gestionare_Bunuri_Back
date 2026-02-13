using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DbTables;
using Domain.Notification;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Asset
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateExpiringNotificationsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            // Curățăm notificările citite vechi (expirate de mai mult de 90 zile)
            // Acestea nu mai sunt relevante și ocupă spațiu inutil
            var cleanupThreshold = now.AddDays(-90);
            var oldReadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId &&
                            n.IsRead &&
                            n.ExpiryDate.HasValue &&
                            n.ExpiryDate.Value < cleanupThreshold)
                .ToListAsync();

            if (oldReadNotifications.Any())
            {
                _context.Notifications.RemoveRange(oldReadNotifications);
            }

            var assets = await _context.Assets
                .Include(a => a.Warranty)
                .Include(a => a.Insurance)
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId)
                .ToListAsync();

            foreach (var asset in assets)
            {
                // Notificare pentru garanție expirat sau pe cale să expire
                if (asset.Warranty != null)
                {
                    if (asset.Warranty.EndDate < now)
                    {
                        // Verificăm dacă există o notificare citită (ignorată) pentru "a expirat"
                        bool dismissed = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.WARRANTY_EXP &&
                            n.ExpiryDate == asset.Warranty.EndDate &&
                            n.IsExpired && // doar cele de tip "a expirat"
                            n.IsRead);

                        bool existsUnread = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.WARRANTY_EXP &&
                            n.ExpiryDate == asset.Warranty.EndDate &&
                            n.IsExpired && // doar cele de tip "a expirat"
                            !n.IsRead);

                        // Șterge notificările "expiră curând" necitite (au trecut în "a expirat")
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        !n.IsExpired && // cele "expiră curând"
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!existsUnread && !dismissed)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.WARRANTY_EXP,
                                Message = $"Garanția pentru '{asset.Name}' a expirat.",
                                CreatedAt = now,
                                ExpiryDate = asset.Warranty.EndDate,
                                IsExpired = true // marcăm ca "a expirat"
                            });
                        }
                    }
                    else if (asset.Warranty.EndDate <= threshold)
                    {
                        // Verificăm dacă există o notificare citită (ignorată) pentru "expiră curând"
                        bool dismissed = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.WARRANTY_EXP &&
                            n.ExpiryDate == asset.Warranty.EndDate &&
                            !n.IsExpired && // doar cele de tip "expiră curând"
                            n.IsRead);

                        bool existsUnread = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.WARRANTY_EXP &&
                            n.ExpiryDate == asset.Warranty.EndDate &&
                            !n.IsExpired && // doar cele de tip "expiră curând"
                            !n.IsRead);

                        // Șterge notificările "a expirat" necitite (data s-a schimbat, acum e "expiră curând")
                        var oldExpiredNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        n.IsExpired && // cele "a expirat"
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiredNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiredNotifications);
                        }

                        // Șterge notificările "expiră curând" necitite cu altă dată de expirare
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        !n.IsExpired &&
                                        n.ExpiryDate != asset.Warranty.EndDate &&
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!existsUnread && !dismissed)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.WARRANTY_EXP,
                                Message = $"Garanția pentru '{asset.Name}' expiră curând.",
                                CreatedAt = now,
                                ExpiryDate = asset.Warranty.EndDate,
                                IsExpired = false // marcăm ca "expiră curând"
                            });
                        }
                    }
                    else if (asset.Warranty.EndDate > threshold)
                    {
                        // Dacă garanția nu mai e în perioada de expirare, șterge notificările necitite pentru acest asset și user
                        var oldWarrantyNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldWarrantyNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldWarrantyNotifications);
                        }
                    }
                }

                // Notificare pentru asigurare expirat sau pe cale să expire
                if (asset.Insurance != null)
                {
                    if (asset.Insurance.EndDate < now)
                    {
                        // Verificăm dacă există o notificare citită (ignorată) pentru "a expirat"
                        bool dismissed = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.INSURANCE_EXP &&
                            n.ExpiryDate == asset.Insurance.EndDate &&
                            n.IsExpired && // doar cele de tip "a expirat"
                            n.IsRead);

                        bool existsUnread = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.INSURANCE_EXP &&
                            n.ExpiryDate == asset.Insurance.EndDate &&
                            n.IsExpired && // doar cele de tip "a expirat"
                            !n.IsRead);

                        // Șterge notificările "expiră curând" necitite (au trecut în "a expirat")
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.INSURANCE_EXP &&
                                        !n.IsExpired && // cele "expiră curând"
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!existsUnread && !dismissed)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.INSURANCE_EXP,
                                Message = $"Asigurarea pentru '{asset.Name}' a expirat.",
                                CreatedAt = now,
                                ExpiryDate = asset.Insurance.EndDate,
                                IsExpired = true // marcăm ca "a expirat"
                            });
                        }
                    }
                    else if (asset.Insurance.EndDate <= threshold)
                    {
                        // Verificăm dacă există o notificare citită (ignorată) pentru "expiră curând"
                        bool dismissed = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.INSURANCE_EXP &&
                            n.ExpiryDate == asset.Insurance.EndDate &&
                            !n.IsExpired && // doar cele de tip "expiră curând"
                            n.IsRead);

                        bool existsUnread = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.INSURANCE_EXP &&
                            n.ExpiryDate == asset.Insurance.EndDate &&
                            !n.IsExpired && // doar cele de tip "expiră curând"
                            !n.IsRead);

                        // Șterge notificările "a expirat" necitite (data s-a schimbat, acum e "expiră curând")
                        var oldExpiredNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.INSURANCE_EXP &&
                                        n.IsExpired && // cele "a expirat"
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiredNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiredNotifications);
                        }

                        // Șterge notificările "expiră curând" necitite cu altă dată de expirare
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.INSURANCE_EXP &&
                                        !n.IsExpired &&
                                        n.ExpiryDate != asset.Insurance.EndDate &&
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!existsUnread && !dismissed)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.INSURANCE_EXP,
                                Message = $"Asigurarea pentru '{asset.Name}' expiră curând.",
                                CreatedAt = now,
                                ExpiryDate = asset.Insurance.EndDate,
                                IsExpired = false // marcăm ca "expiră curând"
                            });
                        }
                    }
                    else if (asset.Insurance.EndDate > threshold)
                    {
                        // Dacă asigurarea nu mai e în perioada de expirare, șterge notificările necitite pentru acest asset și user
                        var oldInsuranceNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.INSURANCE_EXP &&
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldInsuranceNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldInsuranceNotifications);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.Asset)
                .Where(n => n.UserId == userId && !n.IsRead) // Doar notificările necitite
                .ToListAsync();

            var now = DateTime.UtcNow;
            var result = notifications
                .Select(n => new
                {
                    Notification = n,
                    DaysLeft = n.ExpiryDate.HasValue
                        ? (n.ExpiryDate.Value.Date - now.Date).Days
                        : 0
                })
                .OrderByDescending(x => !x.Notification.IsExpired) // Mai întâi cele care expiră curând
                .ThenBy(x => !x.Notification.IsExpired ? x.DaysLeft : int.MaxValue) // Sortăm cele "expiring" după zile rămase
                .ThenByDescending(x => x.Notification.CreatedAt)
                .Select(x => new NotificationDto
                {
                    Id = x.Notification.Id,
                    Type = x.Notification.Type,
                    Message = (x.Notification.Type == NotificationType.WARRANTY_EXP || x.Notification.Type == NotificationType.INSURANCE_EXP) && x.Notification.ExpiryDate.HasValue
                        ? (!x.Notification.IsExpired 
                            ? $"{(x.Notification.Type == NotificationType.WARRANTY_EXP ? "Garanția" : "Asigurarea")} pentru '{x.Notification.Asset.Name}' expiră în {x.DaysLeft} zile."
                            : $"{(x.Notification.Type == NotificationType.WARRANTY_EXP ? "Garanția" : "Asigurarea")} pentru '{x.Notification.Asset.Name}' a expirat.")
                        : x.Notification.Message,
                    IsRead = x.Notification.IsRead,
                    CreatedAt = x.Notification.CreatedAt
                })
                .ToList();

            return result;
        }




        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            // În loc să ștergem, marcăm ca citită (ignorată)
            // Astfel, la regenerare nu se va crea din nou pentru aceeași dată de expirare
            notification.IsRead = true;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
