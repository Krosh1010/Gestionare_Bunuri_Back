using Domain.CoverageStatus;
using Domain.Insurance;
using Infrastructure.Abstraction.CoverageStatus;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Domain.AssetDto;

namespace Infrastructure.Asset.CoverageStatus
{
    public class InsuranceStatusRepository : IInsuranceStatusRepository
    {
        private readonly AppDbContext _context;

        public InsuranceStatusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            var insurances = await _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId)
                .ToListAsync();

            int total = insurances.Count;
            int expired = insurances.Count(i => i.EndDate < now);
            int expiringSoon = insurances.Count(i => i.EndDate >= now && i.EndDate <= threshold);
            int validMoreThanMonth = insurances.Count(i => i.EndDate > threshold);
            decimal totalInsuredValue = insurances.Sum(i => i.InsuredValue);

            int assetsWithoutInsurance = await _context.Assets
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId && a.Insurance == null)
                .CountAsync();

            return new InsuranceSummaryDto
            {
                TotalCount = total,
                ExpiredCount = expired,
                ExpiringSoonCount = expiringSoon,
                ValidMoreThanMonthCount = validMoreThanMonth,
                AssetsWithoutInsuranceCount = assetsWithoutInsurance,
                TotalInsuredValue = totalInsuredValue
            };
        }

        public async Task<PagedResult<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var query = _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId && i.EndDate < now);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(i => i.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ExpiredInsuranceAssetDto
                {
                    AssetName = i.Asset.Name,
                    Category = i.Asset.Category,
                    Company = i.Company,
                    Value = i.InsuredValue,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate
                })
                .ToListAsync();

            return new PagedResult<ExpiredInsuranceAssetDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            var query = _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId && i.EndDate >= now && i.EndDate <= threshold);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ExpiringInsuranceAssetDto
                {
                    AssetName = i.Asset.Name,
                    Category = i.Asset.Category,
                    Company = i.Company,
                    StartDate = i.StartDate,
                    Value = i.InsuredValue,
                    EndDate = i.EndDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, i.EndDate)
                })
                .ToListAsync();

            return new PagedResult<ExpiringInsuranceAssetDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId, int page, int pageSize)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            var query = _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId && i.EndDate > threshold);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.EndDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ValidInsuranceAssetDto
                {
                    AssetName = i.Asset.Name,
                    Category = i.Asset.Category,
                    Company = i.Company,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, i.EndDate),
                    Value = i.InsuredValue
                })
                .ToListAsync();

            return new PagedResult<ValidInsuranceAssetDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        public async Task<PagedResult<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId, int page, int pageSize)
        {
            var query = _context.Assets
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId && a.Insurance == null);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AssetWithoutInsuranceDto
                {
                    AssetName = a.Name,
                    Category = a.Category
                })
                .ToListAsync();

            return new PagedResult<AssetWithoutInsuranceDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
