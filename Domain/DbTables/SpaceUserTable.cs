using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum SpaceUserRole
    {
        OWNER,
        EDITOR,
        VIEWER
    }

    public class SpaceUserTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SpaceId { get; set; }
        public SpaceTable Space { get; set; } = null!;

        [Required]
        public int UserId { get; set; }
        public UserTable User { get; set; } = null!;

        [Required]
        public SpaceUserRole Role { get; set; }
    }
}