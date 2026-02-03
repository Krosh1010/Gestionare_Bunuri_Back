using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum WarrantyStatus
    {
        ACTIVE,
        EXPIRED
    }

    public class WarrantyTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Provider { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [NotMapped]
        public WarrantyStatus Status
        {
            get
            {
                var now = DateTime.UtcNow;
                return now >= StartDate && now <= EndDate
                    ? WarrantyStatus.ACTIVE
                    : WarrantyStatus.EXPIRED;
            }
        }
    }
}
