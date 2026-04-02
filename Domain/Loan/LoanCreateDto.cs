using System.ComponentModel.DataAnnotations;

namespace Domain.Loan
{
    public class LoanCreateDto
    {
        [Required]
        public int AssetId { get; set; }

        [Required, MaxLength(255)]
        public string LoanedToName { get; set; } = null!;

        [Required, MaxLength(500)]
        public string Condition { get; set; } = null!;

        public string? Notes { get; set; }

        [Required]
        public DateTime LoanedAt { get; set; }
    }
}
