using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoverageStatus
{
    public class ExpiringWarrantyAssetDto
    {
        public string AssetName { get; set; } = null!;
        public string? Category { get; set; }
        public string Provider { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysLeft { get; set; }
    }
}
