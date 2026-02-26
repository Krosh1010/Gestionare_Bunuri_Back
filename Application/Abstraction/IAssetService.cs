
using Domain.AssetDto;

public interface IAssetService
{
    Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto);
    Task<AssetReadDto?> GetAssetByIdAsync(int id);
    Task<PagedResult<AssetReadDto>> GetAssetsByUserIdPagedAsync(int userId, AssetPagedRequest request);
    Task<bool> DeleteAssetAsync(int id);
    Task<AssetReadDto?> PatchAssetAsync(int assetId, AssetUpdateDto dto);
    Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId);
}