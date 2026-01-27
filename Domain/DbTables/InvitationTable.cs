using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum InvitationRole
    {
        OWNER,
        EDITOR,
        VIEWER
    }

    public class InvitationTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SpaceId { get; set; }
        public SpaceTable Space { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Token { get; set; } = null!;

        [Required]
        public InvitationRole Role { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;
    }
}