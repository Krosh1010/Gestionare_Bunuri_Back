using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoverageStatus
{
    public class ValidWarrantyAssetDto
    {
        public string AssetName { get; set; } = null!;
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysLeft { get; set; }
    }
}
