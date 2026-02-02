using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
