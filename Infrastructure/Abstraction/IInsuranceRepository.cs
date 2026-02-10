using Domain.Insurance;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction
{
    public interface IInsuranceRepository
    {
        Task CreateInsuranceAsync(InsuranceCreateDto dto);
        Task<InsuranceReadDto?> GetInsuranceByAssetIdAsync(int assetId);
        Task<bool> DeleteInsuranceByAssetIdAsync(int assetId);
        Task<InsuranceReadDto?> PatchInsuranceByAssetIdAsync(int assetId, InsuranceUpdateDto dto);
    }
}
