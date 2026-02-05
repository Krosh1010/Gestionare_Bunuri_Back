using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dashboard
{
    public class DashboardAssetSummaryDto
    {
        public int TotalCount { get; set; }
        public int ElectronicsCount { get; set; }
        public int FurnitureCount { get; set; }
        public int VehiclesCount { get; set; }
        public int DocumentsCount { get; set; }
        public int OtherCount { get; set; }

        // Warranty summary
        public int TotalWarranty { get; set; }
        public int ExpiredWarranty { get; set; }
        public int ExpiringSoonWarranty { get; set; }
        public int ActiveWarranty { get; set; }

        // Insurance summary
        public int TotalInsurance { get; set; }
        public int ExpiredInsurance { get; set; }
        public int ExpiringSoonInsurance { get; set; }
        public int ActiveInsurance { get; set; }

    }
}
