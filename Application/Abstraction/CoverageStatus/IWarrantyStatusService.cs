using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.AssetDto;
using Domain.CoverageStatus;
using Domain.Warranty;

namespace Application.Abstraction.CoverageStatus
{
    public interface IWarrantyStatusService
    {
        Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId);
        Task<PagedResult<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId, int page, int pageSize);
        Task<PagedResult<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId, int page, int pageSize);
    }
}
