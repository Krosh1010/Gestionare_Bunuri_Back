using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DbTables;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
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
                // Pentru garanție expirat
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

                        bool exists = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.WARRANTY_EXP &&
                            n.Message.Contains("expiră în") &&
                            !n.IsRead);

                        if (!exists && !ignored)
                        {
                            var daysLeft = (asset.Warranty.EndDate - now).Days;
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.WARRANTY_EXP,
                                Message = $"Garanția pentru '{asset.Name}' expiră în {daysLeft} zile.",
                                CreatedAt = now
                            });
                        }
                    }
                }


                // Notificare pentru asigurare expirat sau pe cale să expire
                if (asset.Insurance != null)
                {
                    // 1. Notificare "a expirat"
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
                    // 2. Notificare "expiră în X zile"
                    else if (asset.Insurance.EndDate <= threshold)
                    {
                        bool ignored = await _context.IgnoredNotifications.AnyAsync(i =>
                            i.UserId == userId &&
                            i.AssetId == asset.Id &&
                            i.Type == NotificationType.INSURANCE_EXP &&
                            i.MessageType == "expiring");

                        bool exists = await _context.Notifications.AnyAsync(n =>
                            n.AssetId == asset.Id &&
                            n.UserId == userId &&
                            n.Type == NotificationType.INSURANCE_EXP &&
                            n.Message.Contains("expiră în") &&
                            !n.IsRead);

                        if (!exists && !ignored)
                        {
                            var daysLeft = (asset.Insurance.EndDate - now).Days;
                            _context.Notifications.Add(new NotificationTable
                            {
                                UserId = userId,
                                AssetId = asset.Id,
                                Type = NotificationType.INSURANCE_EXP,
                                Message = $"Asigurarea pentru '{asset.Name}' expiră în {daysLeft} zile.",
                                CreatedAt = now
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
        }


        public async Task<List<NotificationTable>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.Asset)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            string messageType = notification.Message.Contains("expiră în") ? "expiring" : "expired";

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

