
namespace Domain.Insurance
{
    public class InsuranceUpdateDto
    {
        public int? SpaceId { get; set; }
        public bool SpaceIdIsSet { get; set; }
        public string? Company { get; set; }
        public decimal? InsuredValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
