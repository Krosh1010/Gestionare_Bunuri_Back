using Application.Abstraction.CoverageStatus;
using Domain.CoverageStatus;
using Domain.Insurance.Domain.Insurance;
using Infrastructure.Abstraction.CoverageStatus;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InsuranceStatusService : IInsuranceStatusService
{
    private readonly IInsuranceStatusRepository _insuranceStatusRepository;

    public InsuranceStatusService(IInsuranceStatusRepository insuranceStatusRepository)
    {
        _insuranceStatusRepository = insuranceStatusRepository;
    }

    public async Task<InsuranceSummaryDto> GetInsuranceSummaryAsync(int userId)
    {
        return await _insuranceStatusRepository.GetInsuranceSummaryAsync(userId);
    }

    public async Task<IEnumerable<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId)
    {
        return await _insuranceStatusRepository.GetExpiredInsuranceAssetsAsync(userId);
    }

    public async Task<IEnumerable<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId)
    {
        return await _insuranceStatusRepository.GetExpiringInsuranceAssetsAsync(userId);
    }

    public async Task<IEnumerable<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId)
    {
        return await _insuranceStatusRepository.GetValidInsuranceAssetsAsync(userId);
    }

    public async Task<IEnumerable<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId)
    {
        return await _insuranceStatusRepository.GetAssetsWithoutInsuranceAsync(userId);
    }
}

