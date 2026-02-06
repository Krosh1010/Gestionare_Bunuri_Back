using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.CoverageStatus;
using Domain.Insurance.Domain.Insurance;

namespace Application.Abstraction.CoverageStatus
{
    public interface IInsuranceStatusService
    {
        Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId);
        Task<IEnumerable<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId);
        Task<IEnumerable<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId);
        Task<IEnumerable<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId);
        Task<IEnumerable<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId);



    }
}
