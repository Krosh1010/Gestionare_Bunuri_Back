using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public class AssetTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int SpaceId { get; set; }
        public SpaceTable Space { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(255)]
        public string? Category { get; set; }

        public decimal Value { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public WarrantyTable? Warranty { get; set; }
        public InsuranceTable? Insurance { get; set; }
        public ICollection<DocumentTable> Documents { get; set; } = new List<DocumentTable>();
        public ICollection<NotificationTable> Notifications { get; set; } = new List<NotificationTable>();
        public ICollection<InsuranceSuggestionTable> InsuranceSuggestions { get; set; } = new List<InsuranceSuggestionTable>();
    }
}