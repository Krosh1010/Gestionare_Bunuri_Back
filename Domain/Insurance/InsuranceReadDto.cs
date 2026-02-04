using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DbTables;

namespace Domain.Insurance
{
    public class InsuranceReadDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string Company { get; set; } = null!;
        public decimal InsuredValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public InsuranceStatus Status { get; set; }
    }

}
