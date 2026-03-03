
using Domain.AssetDto;

public interface IAssetService
{
    Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto);
    Task<AssetReadDto?> GetAssetByIdAsync(int id);
    Task<PagedResult<AssetListDto>> GetAssetsByUserIdPagedAsync(int userId, AssetPagedRequest request);
    Task<bool> DeleteAssetAsync(int id);
    Task<bool> PatchAssetAsync(int assetId, AssetUpdateDto dto);
    Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId);
}