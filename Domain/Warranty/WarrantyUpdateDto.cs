
namespace Domain.Warranty
{
    public class WarrantyUpdateDto
    {
        public int? SpaceId { get; set; }
        public bool SpaceIdIsSet { get; set; }
        public string? Provider { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
