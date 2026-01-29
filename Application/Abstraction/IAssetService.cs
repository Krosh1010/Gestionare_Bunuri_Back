using Domain.AssetDto;

public interface IAssetService
{
    Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto);
    Task<IEnumerable<AssetReadDto>> GetAssetsAsync();
    Task<AssetReadDto?> GetAssetByIdAsync(int id);
    Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId);
    Task<bool> DeleteAssetAsync(int id);
}