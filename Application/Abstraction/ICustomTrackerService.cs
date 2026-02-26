using Domain.CustomTracker;

namespace Application.Abstraction
{
    public interface ICustomTrackerService
    {
        Task<CustomTrackerReadDto> CreateAsync(CustomTrackerCreateDto dto);
        Task<List<CustomTrackerReadDto>> GetByAssetIdAsync(int assetId);
        Task<CustomTrackerReadDto?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<CustomTrackerReadDto?> PatchAsync(int id, CustomTrackerUpdateDto dto);
    }
}
