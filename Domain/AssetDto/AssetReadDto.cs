using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DbTables;

namespace Domain.AssetDto
{
    public class AssetReadDto
    {
        public int Id { get; set; }
        public int SpaceId { get; set; }
        public string SpaceName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Value { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Adaugă aceste proprietăți:
        public DateTime? WarrantyEndDate { get; set; }
        public WarrantyStatus? WarrantyStatus { get; set; }

        public DateTime? InsuranceEndDate { get; set; }
        public InsuranceStatus? InsuranceStatus { get; set; }
    }
}
