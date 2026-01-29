using Domain.AssetDto;
using Domain.DbTables;
using Domain.Space;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

public class SpaceService : ISpaceService
{
    private readonly AppDbContext _context;

    public SpaceService(AppDbContext context)
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

}
