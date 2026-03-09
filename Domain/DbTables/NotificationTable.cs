using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum NotificationType
    {
        WARRANTY_EXP,
        INSURANCE_EXP,
        CUSTOM_TRACKER_EXP
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

        // Opțional: referință la custom tracker (doar pentru CUSTOM_TRACKER_EXP)
        public int? CustomTrackerId { get; set; }
        public CustomTrackerTable? CustomTracker { get; set; }

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

        /// <summary>
        /// true dacă notificarea push a fost deja trimisă pe dispozitiv
        /// </summary>
        public bool IsPushSent { get; set; } = false;

        /// <summary>
        /// true dacă notificarea a fost deja trimisă pe email
        /// </summary>
        public bool IsEmailSent { get; set; } = false;
    }
}