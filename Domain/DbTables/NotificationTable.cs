using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum NotificationType
    {
        WARRANTY_EXP,
        INSURANCE_EXP
    }

    public class NotificationTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public UserTable User { get; set; } = null!;

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        [Required]
        public NotificationType Type { get; set; }

        [Required, MaxLength(500)]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Data de expirare a garanției/asigurării
        public DateTime? ExpiryDate { get; set; }

        // true = notificare "a expirat", false = notificare "expiră curând"
        public bool IsExpired { get; set; } = false;
    }
}