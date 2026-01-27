using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum SpaceType
    {
        HOME,
        OFFICE,
        ROOM,
        STORAGE
    }

    public class SpaceTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; } = null!;

        public int? ParentSpaceId { get; set; }
        public SpaceTable? ParentSpace { get; set; }
        public ICollection<SpaceTable> ChildSpaces { get; set; } = new List<SpaceTable>();

        [Required]
        public int OwnerId { get; set; }
        public UserTable Owner { get; set; } = null!;

        [Required]
        public SpaceType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SpaceUserTable> SpaceUsers { get; set; } = new List<SpaceUserTable>();
        public ICollection<AssetTable> Assets { get; set; } = new List<AssetTable>();
        public ICollection<InvitationTable> Invitations { get; set; } = new List<InvitationTable>();
    }
}