using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public class DeviceTokenTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public UserTable User { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Token { get; set; } = null!;

        /// <summary>
        /// "android" or "ios"
        /// </summary>
        [MaxLength(20)]
        public string Platform { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUsedAt { get; set; }
    }
}
