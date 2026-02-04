using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warranty
{
    public class WarrantySummaryDto
    {
        public int TotalCount { get; set; }
        public int ExpiredCount { get; set; }
        public int ExpiringSoonCount { get; set; } // mai puțin de o lună
        public int ValidMoreThanMonthCount { get; set; }// mai mult de o lună
        public int AssetsWithoutWarrantyCount { get; set; }
    }

}
