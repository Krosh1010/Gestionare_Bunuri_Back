using Application.Abstraction.CoverageStatus;
using Domain.AssetDto;
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

    public async Task<PagedResult<ExpiredWarrantyAssetDto>> GetExpiredWarrantyAssetsAsync(int userId, int page, int pageSize)
    {
        return await _warrantyStatusRepository.GetExpiredWarrantyAssetsAsync(userId, page, pageSize);
    }

    public async Task<PagedResult<ExpiringWarrantyAssetDto>> GetExpiringWarrantyAssetsAsync(int userId, int page, int pageSize)
    {
        return await _warrantyStatusRepository.GetExpiringWarrantyAssetsAsync(userId, page, pageSize);
    }

    public async Task<PagedResult<ValidWarrantyAssetDto>> GetValidWarrantyAssetsAsync(int userId, int page, int pageSize)
    {
        return await _warrantyStatusRepository.GetValidWarrantyAssetsAsync(userId, page, pageSize);
    }

    public async Task<PagedResult<AssetWithoutWarrantyDto>> GetAssetsWithoutWarrantyAsync(int userId, int page, int pageSize)
    {
        return await _warrantyStatusRepository.GetAssetsWithoutWarrantyAsync(userId, page, pageSize);
    }
}

