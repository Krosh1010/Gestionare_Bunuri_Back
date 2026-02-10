using Domain.Space;
using Domain.DbTables;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Asset
{
    public class SpaceRepository : ISpaceRepository
    {
        private readonly AppDbContext _context;

        public SpaceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> CreateSpaceAsync(SpaceCreateDto dto, int ownerId)
        {
            var exists = await _context.Spaces.AnyAsync(s => s.Name == dto.Name && s.OwnerId == ownerId);
            if (exists)
                return "Space with this name already exists for this owner.";

            var space = new SpaceTable
            {
                Name = dto.Name,
                OwnerId = ownerId,
                Type = dto.Type,
                ParentSpaceId = dto.ParentSpaceId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Spaces.Add(space);
            await _context.SaveChangesAsync();

            return new { space.Id, space.Name, space.OwnerId, space.Type, space.ParentSpaceId };
        }

        public async Task<SpaceListDto?> GetSpaceByIdAsync(int spaceId, int ownerId)
        {
            var space = await _context.Spaces
                .FirstOrDefaultAsync(s => s.Id == spaceId && s.OwnerId == ownerId);
            if (space == null)
                return null;

            return new SpaceListDto
            {
                Id = space.Id,
                Name = space.Name,
                Type = space.Type,
                ParentSpaceId = space.ParentSpaceId,
                ChildrenCount = await _context.Spaces.CountAsync(s => s.ParentSpaceId == space.Id && s.OwnerId == ownerId),
                AssetsCount = await _context.Assets.CountAsync(a => a.SpaceId == space.Id)
            };
        }

        public async Task<List<SpaceListDto>> GetSpacesByParentAsync(int ownerId, int? parentSpaceId)
        {
            var spaces = await _context.Spaces
                .Where(s => s.OwnerId == ownerId && s.ParentSpaceId == parentSpaceId)
                .Select(s => new SpaceListDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Type = s.Type,
                    ParentSpaceId = s.ParentSpaceId,
                    ChildrenCount = _context.Spaces.Count(child => child.ParentSpaceId == s.Id && child.OwnerId == ownerId),
                    AssetsCount = _context.Assets.Count(asset => asset.SpaceId == s.Id)
                })
                .ToListAsync();

            return spaces;
        }

        public async Task<List<SpaceListDto>> GetSpacePathAsync(int spaceId)
        {
            var path = new List<SpaceListDto>();
            SpaceTable? current = await _context.Spaces.FindAsync(spaceId);

            while (current != null)
            {
                path.Add(new SpaceListDto
                {
                    Id = current.Id,
                    Name = current.Name,
                    Type = current.Type,
                    ParentSpaceId = current.ParentSpaceId,
                    ChildrenCount = await _context.Spaces.CountAsync(s => s.ParentSpaceId == current.Id),
                    AssetsCount = await _context.Assets.CountAsync(a => a.SpaceId == current.Id)
                });

                if (current.ParentSpaceId == null)
                    break;

                current = await _context.Spaces.FindAsync(current.ParentSpaceId);
            }

            path.Reverse();
            return path;
        }

        public async Task<bool> DeleteSpaceAsync(int spaceId, int ownerId)
        {
            var space = await _context.Spaces
                .Include(s => s.ChildSpaces)
                .FirstOrDefaultAsync(s => s.Id == spaceId && s.OwnerId == ownerId);

            if (space == null)
                return false;

            await DeleteSpaceRecursive(space);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task DeleteSpaceRecursive(SpaceTable space)
        {
            var children = await _context.Spaces
                .Where(s => s.ParentSpaceId == space.Id)
                .ToListAsync();

            foreach (var child in children)
            {
                await DeleteSpaceRecursive(child);
            }

            _context.Spaces.Remove(space);
        }

        public async Task<SpaceListDto?> PatchSpaceAsync(int spaceId, int ownerId, SpaceUpdateDto dto)
        {
            var space = await _context.Spaces.FirstOrDefaultAsync(s => s.Id == spaceId && s.OwnerId == ownerId);
            if (space == null)
                return null;

            if (dto.Name != null)
                space.Name = dto.Name;
            if (dto.Type.HasValue)
                space.Type = dto.Type.Value;
            if (dto.ParentSpaceIdIsSet)
                space.ParentSpaceId = dto.ParentSpaceId;

            await _context.SaveChangesAsync();

            return new SpaceListDto
            {
                Id = space.Id,
                Name = space.Name,
                Type = space.Type,
                ParentSpaceId = space.ParentSpaceId,
                ChildrenCount = await _context.Spaces.CountAsync(s => s.ParentSpaceId == space.Id && s.OwnerId == ownerId),
                AssetsCount = await _context.Assets.CountAsync(a => a.SpaceId == space.Id)
            };
        }
    }
}
