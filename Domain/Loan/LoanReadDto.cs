namespace Domain.Loan
{
    public class LoanReadDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; } = null!;
        public string LoanedToName { get; set; } = null!;
        public string Condition { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime LoanedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public string? ConditionOnReturn { get; set; }
        public bool IsActive => ReturnedAt == null;
    }
}
