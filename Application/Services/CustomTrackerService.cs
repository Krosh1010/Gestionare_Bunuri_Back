using Application.Abstraction;
using Domain.CustomTracker;
using Infrastructure.Abstraction;

namespace Application.Services
{
    public class CustomTrackerService : ICustomTrackerService
    {
        private readonly ICustomTrackerRepository _repository;

        public CustomTrackerService(ICustomTrackerRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomTrackerReadDto> CreateAsync(CustomTrackerCreateDto dto)
        {
            return await _repository.CreateAsync(dto);
        }

        public async Task<List<CustomTrackerReadDto>> GetByAssetIdAsync(int assetId)
        {
            return await _repository.GetByAssetIdAsync(assetId);
        }

        public async Task<CustomTrackerReadDto?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<CustomTrackerReadDto?> PatchAsync(int id, CustomTrackerUpdateDto dto)
        {
            return await _repository.PatchAsync(id, dto);
        }
    }
}
