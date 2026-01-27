using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public class InsuranceSuggestionTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        public int Score { get; set; }

        public string? Recommendation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}