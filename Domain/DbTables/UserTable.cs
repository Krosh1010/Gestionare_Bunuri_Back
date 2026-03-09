using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public class UserTable

        {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required, MaxLength(255)]
        public string FullName { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Token pentru resetarea parolei
        /// </summary>
        [MaxLength(6)]
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Data expirării token-ului de resetare
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public ICollection<SpaceUserTable> SpaceUsers { get; set; } = new List<SpaceUserTable>();
        public ICollection<NotificationTable> Notifications { get; set; } = new List<NotificationTable>();
        public ICollection<DeviceTokenTable> DeviceTokens { get; set; } = new List<DeviceTokenTable>();
    }
}

