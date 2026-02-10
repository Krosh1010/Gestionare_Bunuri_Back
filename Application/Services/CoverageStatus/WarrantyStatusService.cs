using Application.Abstraction.CoverageStatus;
using Domain.CoverageStatus;
using Domain.Warranty;
using Infrastructure.Abstraction.CoverageStatus;
using System.Collections.Generic;
using System.Threading.Tasks;

public class WarrantyStatusService : IWarrantyStatusService
{
    private readonly IWarrantyStatusRepository _warrantyStatusRepository;

    public WarrantyStatusService(IWarrantyStatusRepository warrantyStatusRepository)
    {
        _warrantyStatusRepository = warrantyStatusRepository;
    }

    public async Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId)
    {
        return await _warrantyStatusRepository.GetWarrantySummaryAsync(userId);
    }

    public async Task<IEnumerable<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId)
    {
        return await _warrantyStatusRepository.GetExpiredWarrantyAssetsAsync(userId);
    }

    public async Task<IEnumerable<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId)
    {
        return await _warrantyStatusRepository.GetExpiringWarrantyAssetsAsync(userId);
    }

    public async Task<IEnumerable<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId)
    {
        return await _warrantyStatusRepository.GetValidWarrantyAssetsAsync(userId);
    }

    public async Task<IEnumerable<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId)
    {
        return await _warrantyStatusRepository.GetAssetsWithoutWarrantyAsync(userId);
    }
}

