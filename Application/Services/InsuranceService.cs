
using Application.Abstraction;
using Domain.DbTables;
using Domain.Insurance;
using Infrastructure.Abstraction;
using Domain.Insurance.Domain.Insurance;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository _insuranceRepository;

        public InsuranceService(IInsuranceRepository insuranceRepository)
        {
            _insuranceRepository = insuranceRepository;
        }

        public async Task CreateInsuranceAsync(InsuranceCreateDto dto)
        {
            await _insuranceRepository.CreateInsuranceAsync(dto);
        }

        public async Task<InsuranceReadDto?> GetInsuranceByAssetIdAsync(int assetId)
        {
            return await _insuranceRepository.GetInsuranceByAssetIdAsync(assetId);
        }

        public async Task<bool> DeleteInsuranceByAssetIdAsync(int assetId)
        {
            return await _insuranceRepository.DeleteInsuranceByAssetIdAsync(assetId);
        }

        public async Task<InsuranceReadDto?> PatchInsuranceByAssetIdAsync(int assetId, InsuranceUpdateDto dto)
        {
            return await _insuranceRepository.PatchInsuranceByAssetIdAsync(assetId, dto);
        }
    }
}
