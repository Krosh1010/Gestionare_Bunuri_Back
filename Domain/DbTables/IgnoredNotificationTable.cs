using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DbTables
{
    public class IgnoredNotificationTable
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public NotificationType Type { get; set; }
        public string MessageType { get; set; } = null!; // "expiring" sau "expired"
        public DateTime IgnoredAt { get; set; } = DateTime.UtcNow;
    }
}
