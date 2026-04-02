namespace Domain.Loan
{
    public class LoanReturnDto
    {
        public string? ConditionOnReturn { get; set; }
        public string? Notes { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public DateTime ReturnedAt { get; set; }
    }
}
