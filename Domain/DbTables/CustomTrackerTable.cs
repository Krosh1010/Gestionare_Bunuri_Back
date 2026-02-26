using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum CustomTrackerStatus
    {
        NotStarted,
        Active,
        ExpiringSoon,
        Expired
    }

    public class CustomTrackerTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public CustomTrackerStatus Status
        {
            get
            {
                var now = DateTime.UtcNow;
                if (now < StartDate)
                    return CustomTrackerStatus.NotStarted;
                if (now > EndDate)
                    return CustomTrackerStatus.Expired;
                if ((EndDate - now).TotalDays <= 30)
                    return CustomTrackerStatus.ExpiringSoon;
                return CustomTrackerStatus.Active;
            }
        }
    }
}
