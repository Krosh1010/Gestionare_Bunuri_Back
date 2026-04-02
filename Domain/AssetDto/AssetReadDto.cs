using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DbTables;

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

        // Adaugă aceste proprietăți:
        public DateTime? WarrantyEndDate { get; set; }
        public WarrantyStatus? WarrantyStatus { get; set; }

        public DateTime? InsuranceEndDate { get; set; }
        public InsuranceStatus? InsuranceStatus { get; set; }
        public decimal? InsuranceValue { get; set; }
        public string? InsuranceCompany { get; set; }
        public DateTime? InsuranceStartDate { get; set; }
        public string? WarrantyProvider { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public int? WarrantyDocumentId { get; set; }
        public string? WarrantyDocumentFileName { get; set; }

        public int? InsuranceDocumentId { get; set; }
        public string? InsuranceDocumentFileName { get; set; }

        // CustomTracker info
        public string? CustomTrackerName { get; set; }
        public DateTime? CustomTrackerEndDate { get; set; }
        public CustomTrackerStatus? CustomTrackerStatus { get; set; }

        // Loan info
        public bool IsLoaned { get; set; }
        public int? LoanId { get; set; }
        public string? LoanedToName { get; set; }
        public string? LoanCondition { get; set; }
        public string? LoanNotes { get; set; }
        public DateTime? LoanedAt { get; set; }
        public DateTime? LoanReturnedAt { get; set; }
        public string? LoanConditionOnReturn { get; set; }
    }
}
