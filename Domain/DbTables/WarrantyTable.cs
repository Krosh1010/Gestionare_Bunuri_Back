using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum WarrantyStatus
    {
        NotStarted,
        Active,
        ExpiringSoon,
        Expired
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
                if (now < StartDate)
                    return WarrantyStatus.NotStarted;
                if (now > EndDate)
                    return WarrantyStatus.Expired;
                if ((EndDate - now).TotalDays <= 30)
                    return WarrantyStatus.ExpiringSoon;
                return WarrantyStatus.Active;
            }
        }
    }
}
