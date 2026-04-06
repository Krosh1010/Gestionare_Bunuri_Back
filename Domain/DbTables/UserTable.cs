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

        public bool IsEmailVerified { get; set; } = false;

        [MaxLength(6)]
        public string? EmailVerificationToken { get; set; }

        public DateTime? EmailVerificationTokenExpiry { get; set; }

        [MaxLength(6)]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        public ICollection<SpaceUserTable> SpaceUsers { get; set; } = new List<SpaceUserTable>();
        public ICollection<NotificationTable> Notifications { get; set; } = new List<NotificationTable>();
        public ICollection<DeviceTokenTable> DeviceTokens { get; set; } = new List<DeviceTokenTable>();
    }
}

