using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstraction;
using Domain.DbTables;
using Domain.Insurance;
using Domain.Insurance.Domain.Insurance;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly AppDbContext _context;

        public InsuranceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateInsuranceAsync(InsuranceCreateDto dto)
        {
            var insurance = new InsuranceTable
            {
                AssetId = dto.AssetId,
                Company = dto.Company,
                InsuredValue = dto.InsuredValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.Insurances.Add(insurance);
            await _context.SaveChangesAsync();
            // Nu mai returnezi nimic
        }

        public async Task<InsuranceReadDto?> GetInsuranceByAssetIdAsync(int assetId)
        {
            var insurance = await _context.Insurances
                .FirstOrDefaultAsync(i => i.AssetId == assetId);

            if (insurance == null)
                return null;

            return new InsuranceReadDto
            {
                Id = insurance.Id,
                AssetId = insurance.AssetId,
                Company = insurance.Company,
                InsuredValue = insurance.InsuredValue,
                StartDate = insurance.StartDate,
                EndDate = insurance.EndDate,
                Status = insurance.Status
            };
        }
        public async Task<bool> DeleteInsuranceByAssetIdAsync(int assetId)
        {
            var insurance = await _context.Insurances.FirstOrDefaultAsync(i => i.AssetId == assetId);
            if (insurance == null)
                return false;

            _context.Insurances.Remove(insurance);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<InsuranceReadDto?> PatchInsuranceByAssetIdAsync(int assetId, InsuranceUpdateDto dto)
        {
            var insurance = await _context.Insurances.FirstOrDefaultAsync(i => i.AssetId == assetId);
            if (insurance == null)
                return null;

            if (dto.Company != null)
                insurance.Company = dto.Company;
            if (dto.InsuredValue.HasValue)
                insurance.InsuredValue = dto.InsuredValue.Value;
            if (dto.StartDate.HasValue)
                insurance.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue)
                insurance.EndDate = dto.EndDate.Value;

            await _context.SaveChangesAsync();

            return new InsuranceReadDto
            {
                Id = insurance.Id,
                AssetId = insurance.AssetId,
                Company = insurance.Company,
                InsuredValue = insurance.InsuredValue,
                StartDate = insurance.StartDate,
                EndDate = insurance.EndDate,
                Status = insurance.Status
            };
        }

        public async Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var oneMonthLater = now.AddMonths(1);

            // Ia toate asset-urile userului
            var assets = await _context.Assets
                .Include(a => a.Insurance)
                .Include(a => a.Space)
                .Where(a => a.Space.OwnerId == userId)
                .ToListAsync();

            // Ia doar asigurările existente
            var insurances = assets
                .Where(a => a.Insurance != null)
                .Select(a => a.Insurance!)
                .ToList();

            int total = insurances.Count;
            int expired = insurances.Count(i => i.EndDate < now);
            int expiringSoon = insurances.Count(i => i.EndDate >= now && i.EndDate <= oneMonthLater);
            int validMoreThanMonth = insurances.Count(i => i.EndDate > oneMonthLater);
            int assetsWithoutInsurance = assets.Count(a => a.Insurance == null);

            return new InsuranceSummaryDto
            {
                TotalCount = total,
                ExpiredCount = expired,
                ExpiringSoonCount = expiringSoon,
                ValidMoreThanMonthCount = validMoreThanMonth,
                AssetsWithoutInsuranceCount = assetsWithoutInsurance
            };
        }


    }
}
