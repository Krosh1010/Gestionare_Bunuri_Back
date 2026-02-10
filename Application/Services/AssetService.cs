using Domain.AssetDto;
using Application.Abstraction;
using Infrastructure.Abstraction;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;

    public AssetService(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto)
    {
        return await _assetRepository.CreateAssetAsync(dto);
    }

    public async Task<AssetReadDto?> GetAssetByIdAsync(int id)
    {
        return await _assetRepository.GetAssetByIdAsync(id);
    }

    public async Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId)
    {
        return await _assetRepository.GetAssetsByUserIdAsync(userId);
    }

    public async Task<bool> DeleteAssetAsync(int id)
    {
        return await _assetRepository.DeleteAssetAsync(id);
    }

    public async Task<AssetReadDto?> PatchAssetAsync(int assetId, AssetUpdateDto dto)
    {
        return await _assetRepository.PatchAssetAsync(assetId, dto);
    }
}
