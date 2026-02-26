using Domain.CustomTracker;
using Domain.DbTables;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Asset
{
    public class CustomTrackerRepository : ICustomTrackerRepository
    {
        private readonly AppDbContext _context;

        public CustomTrackerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomTrackerReadDto> CreateAsync(CustomTrackerCreateDto dto)
        {
            var tracker = new CustomTrackerTable
            {
                AssetId = dto.AssetId,
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.CustomTrackers.Add(tracker);
            await _context.SaveChangesAsync();

            return new CustomTrackerReadDto
            {
                Id = tracker.Id,
                AssetId = tracker.AssetId,
                Name = tracker.Name,
                Description = tracker.Description,
                StartDate = tracker.StartDate,
                EndDate = tracker.EndDate,
                Status = tracker.Status,
                CreatedAt = tracker.CreatedAt
            };
        }

        public async Task<List<CustomTrackerReadDto>> GetByAssetIdAsync(int assetId)
        {
            return await _context.CustomTrackers
                .Where(ct => ct.AssetId == assetId)
                .OrderByDescending(ct => ct.CreatedAt)
                .Select(ct => new CustomTrackerReadDto
                {
                    Id = ct.Id,
                    AssetId = ct.AssetId,
                    Name = ct.Name,
                    Description = ct.Description,
                    StartDate = ct.StartDate,
                    EndDate = ct.EndDate,
                    Status = ct.Status,
                    CreatedAt = ct.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CustomTrackerReadDto?> GetByIdAsync(int id)
        {
            var tracker = await _context.CustomTrackers.FindAsync(id);
            if (tracker == null)
                return null;

            return new CustomTrackerReadDto
            {
                Id = tracker.Id,
                AssetId = tracker.AssetId,
                Name = tracker.Name,
                Description = tracker.Description,
                StartDate = tracker.StartDate,
                EndDate = tracker.EndDate,
                Status = tracker.Status,
                CreatedAt = tracker.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tracker = await _context.CustomTrackers.FindAsync(id);
            if (tracker == null)
                return false;

            _context.CustomTrackers.Remove(tracker);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CustomTrackerReadDto?> PatchAsync(int id, CustomTrackerUpdateDto dto)
        {
            var tracker = await _context.CustomTrackers.FindAsync(id);
            if (tracker == null)
                return null;

            if (dto.Name != null)
                tracker.Name = dto.Name;
            if (dto.Description != null)
                tracker.Description = dto.Description;
            if (dto.StartDate.HasValue)
                tracker.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue)
                tracker.EndDate = dto.EndDate.Value;

            await _context.SaveChangesAsync();

            return new CustomTrackerReadDto
            {
                Id = tracker.Id,
                AssetId = tracker.AssetId,
                Name = tracker.Name,
                Description = tracker.Description,
                StartDate = tracker.StartDate,
                EndDate = tracker.EndDate,
                Status = tracker.Status,
                CreatedAt = tracker.CreatedAt
            };
        }
    }
}
