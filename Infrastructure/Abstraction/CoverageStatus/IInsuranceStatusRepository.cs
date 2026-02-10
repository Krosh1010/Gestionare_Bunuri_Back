using Domain.CoverageStatus;
using Domain.Insurance.Domain.Insurance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction.CoverageStatus
{
    public interface IInsuranceStatusRepository
    {
        Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId);
        Task<IEnumerable<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId);
        Task<IEnumerable<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId);
        Task<IEnumerable<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId);
        Task<IEnumerable<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId);
    }
}
