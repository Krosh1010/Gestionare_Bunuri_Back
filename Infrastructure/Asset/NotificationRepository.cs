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
                        bool ignored = await _context.IgnoredNotifications.AnyAsync(i =>
                            i.UserId == userId &&
                            i.AssetId == asset.Id &&
                            i.Type == NotificationType.WARRANTY_EXP &&
                            i.MessageType == "expired");

                        bool exists = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.WARRANTY_EXP &&
                            n.Message.Contains("a expirat") &&
                            !n.IsRead);

                        // Șterge notificările "expiră curând" necitite pentru acest asset și user
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        n.ExpiryDate != null && // doar cele cu ExpiryDate (adică "expiră curând")
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!exists && !ignored)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.WARRANTY_EXP,
                                Message = $"Garanția pentru '{asset.Name}' a expirat.",
                                CreatedAt = now
                            });
                        }
                    }
                    else if (asset.Warranty.EndDate <= threshold)
                    {
                        bool ignored = await _context.IgnoredNotifications.AnyAsync(i =>
                            i.UserId == userId &&
                            i.AssetId == asset.Id &&
                            i.Type == NotificationType.WARRANTY_EXP &&
                            i.MessageType == "expiring");

                        // Șterge notificările "a expirat" necitite pentru acest asset și user
                        var oldExpiredNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        n.ExpiryDate == null && // doar cele "a expirat"
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiredNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiredNotifications);
                        }

                        // Șterge notificările "expiră curând" duplicate necitite pentru acest asset și user
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.WARRANTY_EXP &&
                                        n.ExpiryDate != null &&
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!ignored)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.WARRANTY_EXP,
                                Message = $"Garanția pentru '{asset.Name}' expiră curând.",
                                CreatedAt = now,
                                ExpiryDate = asset.Warranty.EndDate
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
                        bool ignored = await _context.IgnoredNotifications.AnyAsync(i =>
                            i.UserId == userId &&
                            i.AssetId == asset.Id &&
                            i.Type == NotificationType.INSURANCE_EXP &&
                            i.MessageType == "expired");

                        bool exists = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.INSURANCE_EXP &&
                            n.Message.Contains("a expirat") &&
                            !n.IsRead);
                        // Șterge notificările "expiră curând" necitite pentru acest asset și user
                        var oldExpiringNotifications = await _context.Notifications
                            .Where(n => n.AssetId == asset.Id &&
                                        n.UserId == userId &&
                                        n.Type == NotificationType.INSURANCE_EXP &&
                                        n.ExpiryDate != null && // doar cele cu ExpiryDate (adică "expiră curând")
                                        !n.IsRead)
                            .ToListAsync();

                        if (oldExpiringNotifications.Any())
                        {
                            _context.Notifications.RemoveRange(oldExpiringNotifications);
                        }

                        if (!exists && !ignored)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.INSURANCE_EXP,
                                Message = $"Asigurarea pentru '{asset.Name}' a expirat.",
                                CreatedAt = now
                            });
                        }
                    }
                    else if (asset.Insurance.EndDate <= threshold)
                    {
                        bool ignored = await _context.IgnoredNotifications.AnyAsync(i =>
                            i.UserId == userId &&
                            i.AssetId == asset.Id &&
                            i.Type == NotificationType.INSURANCE_EXP &&
                            i.MessageType == "expiring");

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

                        if (!ignored)
                        {
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.INSURANCE_EXP,
                                Message = $"Asigurarea pentru '{asset.Name}' expiră curând.",
                                CreatedAt = now,
                                ExpiryDate = asset.Insurance.EndDate
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
                .Where(n => n.UserId == userId)
                .ToListAsync();

            var result = notifications
                .Select(n => new
                {
                    Notification = n,
                    DaysLeft = n.ExpiryDate.HasValue
                        ? (n.ExpiryDate.Value.Date - DateTime.UtcNow.Date).Days
                        : int.MaxValue // pentru cele fără ExpiryDate (ex: "a expirat"), le punem la final
                })
                .OrderBy(x => x.DaysLeft)
                .ThenByDescending(x => x.Notification.CreatedAt)
                .Select(x => new NotificationDto
                {
                    Id = x.Notification.Id,
                    Type = x.Notification.Type,
                    Message = (x.Notification.Type == NotificationType.WARRANTY_EXP || x.Notification.Type == NotificationType.INSURANCE_EXP) && x.Notification.ExpiryDate.HasValue
                        ? $"{(x.Notification.Type == NotificationType.WARRANTY_EXP ? "Garanția" : "Asigurarea")} pentru '{x.Notification.Asset.Name}' expiră în {x.DaysLeft} zile."
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

            string messageType = notification.ExpiryDate.HasValue ? "expiring" : "expired";

            _context.IgnoredNotifications.Add(new IgnoredNotificationTable
            {
                UserId = notification.UserId,
                AssetId = notification.AssetId,
                Type = notification.Type,
                MessageType = messageType
            });

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
