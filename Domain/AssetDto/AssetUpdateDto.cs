
namespace Domain.AssetDto
{
    public class AssetUpdateDto
    {
        public int? SpaceId { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal? Value { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Description { get; set; }
    }
}
