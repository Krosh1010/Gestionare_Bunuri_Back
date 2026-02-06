using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CoverageStatus
{
    public class AssetWithoutInsuranceDto
    {
        public string AssetName { get; set; } = null!;
        public string? Category { get; set; }
    }
}
