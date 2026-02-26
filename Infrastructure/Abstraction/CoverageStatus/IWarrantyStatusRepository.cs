using Domain.CoverageStatus;
using Domain.Warranty;
using Domain.AssetDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction.CoverageStatus
{
    public interface IWarrantyStatusRepository
    {
        Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId);
        Task<PagedResult<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId, int page, int pageSize);
    }
}
