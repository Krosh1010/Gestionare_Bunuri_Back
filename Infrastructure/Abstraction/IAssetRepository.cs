
using Domain.AssetDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction
{
    public interface IAssetRepository
    {
        Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto);
        Task<AssetReadDto?> GetAssetByIdAsync(int id);
        Task<PagedResult<AssetListDto>> GetAssetsByUserIdPagedAsync(int userId, AssetPagedRequest request);
        Task<bool> DeleteAssetAsync(int id);
        Task<AssetReadDto?> PatchAssetAsync(int assetId, AssetUpdateDto dto);
        Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId);
    }
}
