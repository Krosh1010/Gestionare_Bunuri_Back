using Domain.Space;
using Infrastructure.Abstraction;


public class SpaceService : ISpaceService
{
    private readonly ISpaceRepository _spaceRepository;

    public SpaceService(ISpaceRepository spaceRepository)
    {
        _spaceRepository = spaceRepository;
    }

    public async Task<object> CreateSpaceAsync(SpaceCreateDto dto, int ownerId)
    {
        return await _spaceRepository.CreateSpaceAsync(dto, ownerId);
    }

    public async Task<SpaceListDto?> GetSpaceByIdAsync(int spaceId, int ownerId)
    {
        return await _spaceRepository.GetSpaceByIdAsync(spaceId, ownerId);
    }

    public async Task<List<SpaceListDto>> GetSpacesByParentAsync(int ownerId, int? parentSpaceId)
    {
        return await _spaceRepository.GetSpacesByParentAsync(ownerId, parentSpaceId);
    }

    public async Task<List<SpaceListDto>> GetSpacePathAsync(int spaceId)
    {
        return await _spaceRepository.GetSpacePathAsync(spaceId);
    }

    public async Task<bool> DeleteSpaceAsync(int spaceId, int ownerId)
    {
        return await _spaceRepository.DeleteSpaceAsync(spaceId, ownerId);
    }

    public async Task<SpaceListDto?> PatchSpaceAsync(int spaceId, int ownerId, SpaceUpdateDto dto)
    {
        return await _spaceRepository.PatchSpaceAsync(spaceId, ownerId, dto);
    }
}
            
