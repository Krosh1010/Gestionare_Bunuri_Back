using Domain.Insurance;
using Domain.DbTables;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Asset
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly AppDbContext _context;

        public InsuranceRepository(AppDbContext context)
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
    }
}
