using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoverageStatus
{
    public class ExpiredInsuranceAssetDto
    {
        public string AssetName { get; set; } = null!;
        public string? Category { get; set; }
        public decimal Value { get; set; }
        public string Company { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
