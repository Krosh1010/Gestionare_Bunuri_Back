using Application.Abstraction.CoverageStatus;
using Domain.AssetDto;
using Domain.CoverageStatus;
using Domain.Insurance;
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

    public async Task<PagedResult<ExpiredInsuranceAssetDto>> GetExpiredInsuranceAssetsAsync(int userId, int page, int pageSize)
    {
        return await _insuranceStatusRepository.GetExpiredInsuranceAssetsAsync(userId, page, pageSize);
    }

    public async Task<PagedResult<ExpiringInsuranceAssetDto>> GetExpiringInsuranceAssetsAsync(int userId, int page, int pageSize)
    {
        return await _insuranceStatusRepository.GetExpiringInsuranceAssetsAsync(userId, page, pageSize);
    }

    public async Task<PagedResult<ValidInsuranceAssetDto>> GetValidInsuranceAssetsAsync(int userId, int page, int pageSize)
    {
        return await _insuranceStatusRepository.GetValidInsuranceAssetsAsync(userId, page, pageSize);
    }

    public async Task<PagedResult<AssetWithoutInsuranceDto>> GetAssetsWithoutInsuranceAsync(int userId, int page, int pageSize)
    {
        return await _insuranceStatusRepository.GetAssetsWithoutInsuranceAsync(userId, page, pageSize);
    }
}

