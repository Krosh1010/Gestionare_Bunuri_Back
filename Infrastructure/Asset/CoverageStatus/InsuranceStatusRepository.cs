using Domain.CoverageStatus;
using Domain.Insurance.Domain.Insurance;
using Infrastructure.Abstraction.CoverageStatus;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

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

        public async Task<IEnumerable<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            return await _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId && i.EndDate < now)
                .Select(i => new ExpiredInsuranceAssetDto
                {
                    AssetName = i.Asset.Name,
                    Category = i.Asset.Category,
                    Value = i.InsuredValue,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            return await _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId && i.EndDate >= now && i.EndDate <= threshold)
                .Select(i => new ExpiringInsuranceAssetDto
                {
                    AssetName = i.Asset.Name,
                    Category = i.Asset.Category,
                    StartDate = i.StartDate,
                    Value = i.InsuredValue,
                    EndDate = i.EndDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, i.EndDate)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var threshold = now.AddDays(30);

            return await _context.Insurances
                .Include(i => i.Asset)
                .Where(i => i.Asset.Space.OwnerId == userId && i.EndDate > threshold)
                .Select(i => new ValidInsuranceAssetDto
                {
                    AssetName = i.Asset.Name,
                    Category = i.Asset.Category,
                    StartDate = i.StartDate,
                    EndDate = i.EndDate,
                    DaysLeft = EF.Functions.DateDiffDay(now, i.EndDate),
                    Value = i.InsuredValue
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId)
        {
            return await _context.Assets
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId && a.Insurance == null)
                .Select(a => new AssetWithoutInsuranceDto
                {
                    AssetName = a.Name,
                    Category = a.Category
                })
                .ToListAsync();
        }
    }
}
