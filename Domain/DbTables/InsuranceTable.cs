using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum InsuranceStatus
    {
        NotStarted,
        Active,
        ExpiringSoon,
        Expired
    }

    public class InsuranceTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Company { get; set; } = null!;

        public decimal InsuredValue { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [NotMapped]
        public InsuranceStatus Status
        {
            get
            {
                var now = DateTime.UtcNow;
                if (now < StartDate)
                    return InsuranceStatus.NotStarted;
                if (now > EndDate)
                    return InsuranceStatus.Expired;
                if ((EndDate - now).TotalDays <= 30)
                    return InsuranceStatus.ExpiringSoon;
                return InsuranceStatus.Active;
            }
        }
    }
}