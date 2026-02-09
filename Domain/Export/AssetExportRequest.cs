using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Export
{
    public class AssetExportRequest
    {
        public List<string>? Categories { get; set; }
        public string? InsuranceStatus { get; set; } // "toate", "expirate", "expira_curand", "active"
        public string? WarrantyStatus { get; set; }  // "toate", "expirate", "expira_curand", "active"
        public List<string> Columns { get; set; } = new();
    }
}
