using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DbTables;

namespace Domain.Warranty
{
    public class WarrantyReadDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string Provider { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public WarrantyStatus Status { get; set; }
    }
}
