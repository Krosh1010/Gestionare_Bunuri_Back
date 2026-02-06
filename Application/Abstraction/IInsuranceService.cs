
using Domain.Insurance;
using Domain.Insurance.Domain.Insurance;

namespace Application.Abstraction
{
    public interface IInsuranceService
    {
        Task CreateInsuranceAsync(InsuranceCreateDto dto);
        Task<InsuranceReadDto?> GetInsuranceByAssetIdAsync(int assetId);
        Task<bool> DeleteInsuranceByAssetIdAsync(int assetId);
        Task<InsuranceReadDto?> PatchInsuranceByAssetIdAsync(int assetId, InsuranceUpdateDto dto);
    }

}
