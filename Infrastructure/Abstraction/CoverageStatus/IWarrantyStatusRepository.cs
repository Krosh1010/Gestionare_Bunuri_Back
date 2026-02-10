using Domain.CoverageStatus;
using Domain.Warranty;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction.CoverageStatus
{
    public interface IWarrantyStatusRepository
    {
        Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId);
        Task<IEnumerable<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId);
        Task<IEnumerable<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId);
        Task<IEnumerable<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId);
        Task<IEnumerable<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId);
    }
}
