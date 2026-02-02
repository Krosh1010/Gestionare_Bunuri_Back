using Domain.AssetDto;

public interface IAssetService
{
    Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto);
    Task<AssetReadDto?> GetAssetByIdAsync(int id);
    Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId);
    Task<bool> DeleteAssetAsync(int id);
    Task<AssetReadDto?> PatchAssetAsync(int assetId, AssetUpdateDto dto);
}