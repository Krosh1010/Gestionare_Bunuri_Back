
namespace Domain.Insurance
{
    public class InsuranceUpdateDto
    {
        public string? Company { get; set; }
        public decimal? InsuredValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
