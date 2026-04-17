using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DbTables
{
    public enum DocumentType
    {
        WARRANTY,
        INSURANCE,
        LOAN,
        OTHER
    }

    public class DocumentTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }
        public AssetTable Asset { get; set; } = null!;

        [Required]
        public DocumentType Type { get; set; }

        [Required, MaxLength(255)]
        public string FileName { get; set; } = null!;

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = null!;

        public int? LoanId { get; set; }
        public LoanTable? Loan { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}