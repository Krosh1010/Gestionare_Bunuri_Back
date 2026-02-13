using Domain.CoverageStatus;
using Domain.Warranty;
using Infrastructure.Abstraction.CoverageStatus;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Asset.CoverageStatus
{
    public class WarrantyStatusRepository : IWarrantyStatusRepository
    {
        private readonly AppDbContext _context;

        public WarrantyStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            var warranties = await _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId)
                .ToListAsync();

            int total = warranties.Count;
            int expired = warranties.Count(w => w.EndDate < now);
            int expiringSoon = warranties.Count(w => w.EndDate >= now && w.EndDate <= threshold);
            int validMoreThanMonth = warranties.Count(w => w.EndDate > threshold);

            var assetsWithoutWarranty = await _context.Assets
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId && a.Warranty == null)
                .CountAsync();

            return new WarrantySummaryDto
            {
                TotalCount = total,
                ExpiredCount = expired,
                ExpiringSoonCount = expiringSoon,
                ValidMoreThanMonthCount = validMoreThanMonth,
                AssetsWithoutWarrantyCount = assetsWithoutWarranty
            };
        }

        public async Task<IEnumerable<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId && w.EndDate < now)
                .Select(w => new ExpiredWarrantyAssetDto
                {
                    AssetName = w.Asset.Name,
                    Category = w.Asset.Category,
                    Provider = w.Provider,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            return await _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId && w.EndDate >= now && w.EndDate <= threshold)
                .Select(w => new ExpiringWarrantyAssetDto
                {
                    AssetName = w.Asset.Name,
                    Category = w.Asset.Category,
                    Provider = w.Provider,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, w.EndDate)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            return await _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId && w.EndDate > threshold)
                .Select(w => new ValidWarrantyAssetDto
                {
                    AssetName = w.Asset.Name,
                    Category = w.Asset.Category,
                    Provider = w.Provider,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, w.EndDate)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId)
        {
            return await _context.Assets
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId && a.Warranty == null)
                .Select(a => new AssetWithoutWarrantyDto
                {
                    AssetName = a.Name,
                    Category = a.Category,
                })
                .ToListAsync();
        }
    }
}
