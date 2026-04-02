namespace Domain.Loan
{
    public class LoanUpdateDto
    {
        public string? LoanedToName { get; set; }
        public string? Condition { get; set; }
        public string? Notes { get; set; }
        public DateTime? LoanedAt { get; set; }
    }
}
