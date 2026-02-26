
using Domain.AssetDto;
using Domain.CoverageStatus;
using Domain.Insurance;

namespace Application.Abstraction.CoverageStatus
{
    public interface IInsuranceStatusService
    {
        Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId);
        Task<PagedResult<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId, int page, int pageSize);
    }
}
