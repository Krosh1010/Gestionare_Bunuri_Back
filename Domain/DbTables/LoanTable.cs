using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public class LoanTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        [Required, MaxLength(255)]
        public string LoanedToName { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Condition { get; set; } = null!;

        public string? Notes { get; set; }

        public DateTime LoanedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReturnedAt { get; set; }

        [MaxLength(500)]
        public string? ConditionOnReturn { get; set; }
    }
}
