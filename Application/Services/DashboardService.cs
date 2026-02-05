using System.Collections.Generic;
using Application.Abstraction;
using Domain.Dashboard;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardAssetSummaryDto> GetAssetSummaryAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var oneMonthLater = now.AddMonths(1);

            var assets = await _context.Assets
                .Include(a => a.Warranty)
                .Include(a => a.Insurance)
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId)
                .ToListAsync();

            int total = assets.Count;
            int electronics = assets.Count(a => a.Category != null && a.Category.ToLower() == "electronics");
            int furniture = assets.Count(a => a.Category != null && a.Category.ToLower() == "furniture");
            int vehicles = assets.Count(a => a.Category != null && a.Category.ToLower() == "vehicles");
            int documents = assets.Count(a => a.Category != null && a.Category.ToLower() == "documents");
            int other = assets.Count(a => a.Category != null && a.Category.ToLower() == "other");

            // Warranty
            var warranties = assets.Where(a => a.Warranty != null).Select(a => a.Warranty!).ToList();
            int totalWarranty = warranties.Count;
            int expiredWarranty = warranties.Count(w => w.EndDate < now);
            int expiringSoonWarranty = warranties.Count(w => w.EndDate >= now && w.EndDate <= oneMonthLater);
            int activeWarranty = warranties.Count(w => w.EndDate > oneMonthLater);

            // Insurance
            var insurances = assets.Where(a => a.Insurance != null).Select(a => a.Insurance!).ToList();
            int totalInsurance = insurances.Count;
            int expiredInsurance = insurances.Count(i => i.EndDate < now);
            int expiringSoonInsurance = insurances.Count(i => i.EndDate >= now && i.EndDate <= oneMonthLater);
            int activeInsurance = insurances.Count(i => i.EndDate > oneMonthLater);

            // Combined (optional, keep if needed)
            var warrantyAndInsurance = new List<DateTime?>();
            warrantyAndInsurance.AddRange(warranties.Select(w => (DateTime?)w.EndDate));
            warrantyAndInsurance.AddRange(insurances.Select(i => (DateTime?)i.EndDate));
            int totalWI = warrantyAndInsurance.Count;
            int expiredWI = warrantyAndInsurance.Count(x => x.HasValue && x.Value < now);
            int expiringSoonWI = warrantyAndInsurance.Count(x => x.HasValue && x.Value >= now && x.Value <= oneMonthLater);
            int activeWI = warrantyAndInsurance.Count(x => x.HasValue && x.Value > oneMonthLater);

            return new DashboardAssetSummaryDto
            {
                TotalCount = total,
                ElectronicsCount = electronics,
                FurnitureCount = furniture,
                VehiclesCount = vehicles,
                DocumentsCount = documents,
                OtherCount = other,

                // Warranty
                TotalWarranty = totalWarranty,
                ExpiredWarranty = expiredWarranty,
                ExpiringSoonWarranty = expiringSoonWarranty,
                ActiveWarranty = activeWarranty,

                // Insurance
                TotalInsurance = totalInsurance,
                ExpiredInsurance = expiredInsurance,
                ExpiringSoonInsurance = expiringSoonInsurance,
                ActiveInsurance = activeInsurance,

            };
        }


    }
}
