using Domain.Space;
using Domain.DbTables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Abstraction
{
    public interface ISpaceRepository
    {
        Task<object> CreateSpaceAsync(SpaceCreateDto dto, int ownerId);
        Task<List<SpaceListDto>> GetSpacesByParentAsync(int ownerId, int? parentSpaceId);
        Task<SpaceListDto?> GetSpaceByIdAsync(int spaceId, int ownerId);
        Task<bool> DeleteSpaceAsync(int spaceId, int ownerId);
        Task<List<SpaceListDto>> GetSpacePathAsync(int spaceId);
        Task<SpaceListDto?> PatchSpaceAsync(int spaceId, int ownerId, SpaceUpdateDto dto);
    }
}
