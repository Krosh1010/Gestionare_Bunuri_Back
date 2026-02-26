using Domain.AssetDto;
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

        public async Task<PagedResult<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var query = _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId && w.EndDate < now);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(w => w.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new ExpiredWarrantyAssetDto
                {
                    AssetName = w.Asset.Name,
                    Category = w.Asset.Category,
                    Provider = w.Provider,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate
                })
                .ToListAsync();

            return new PagedResult<ExpiredWarrantyAssetDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            var query = _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId && w.EndDate >= now && w.EndDate <= threshold);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(w => w.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return new PagedResult<ExpiringWarrantyAssetDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            var query = _context.Warranties
                .Include(w => w.Asset)
                .Where(w => w.Asset.Space.OwnerId == userId && w.EndDate > threshold);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(w => w.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return new PagedResult<ValidWarrantyAssetDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId, int page, int pageSize)
        {
            var query = _context.Assets
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId && a.Warranty == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AssetWithoutWarrantyDto
                {
                    AssetName = a.Name,
                    Category = a.Category,
                })
                .ToListAsync();

            return new PagedResult<AssetWithoutWarrantyDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
