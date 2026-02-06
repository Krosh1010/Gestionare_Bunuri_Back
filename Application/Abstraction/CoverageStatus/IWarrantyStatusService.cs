using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.CoverageStatus;
using Domain.Warranty;

namespace Application.Abstraction.CoverageStatus
{
    public interface IWarrantyStatusService
    {
        Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId);
        Task<IEnumerable<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId);
        Task<IEnumerable<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId);
        Task<IEnumerable<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId);
        Task<IEnumerable<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId);


    }
}
