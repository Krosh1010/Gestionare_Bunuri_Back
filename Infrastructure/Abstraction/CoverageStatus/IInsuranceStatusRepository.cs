using Domain.AssetDto;
using Domain.CoverageStatus;
using Domain.Insurance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction.CoverageStatus
{
    public interface IInsuranceStatusRepository
    {
        Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId);
        Task<PagedResult<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId, int page, int pageSize);
    }
}
