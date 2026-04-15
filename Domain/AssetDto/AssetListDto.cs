using Domain.DbTables;

namespace Domain.AssetDto
{
    public class AssetListDto
    {
        public int Id { get; set; }
        public int SpaceId { get; set; }
        public string SpaceName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Value { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Description { get; set; }
        public string? Barcode { get; set; }

        public DateTime? WarrantyEndDate { get; set; }
        public WarrantyStatus? WarrantyStatus { get; set; }

        public DateTime? InsuranceEndDate { get; set; }
        public InsuranceStatus? InsuranceStatus { get; set; }

        public string? CustomTrackerName { get; set; }
        public DateTime? CustomTrackerEndDate { get; set; }
        public CustomTrackerStatus? CustomTrackerStatus { get; set; }

        public bool IsLoaned { get; set; }
    }
}
