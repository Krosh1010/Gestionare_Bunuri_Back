using System.Net.NetworkInformation;
using Domain.AssetDto;
using Domain.DbTables;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

public class AssetService : IAssetService
{
    private readonly AppDbContext _context;

    public AssetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto)
    {
        var asset = new AssetTable
        {
            SpaceId = dto.SpaceId,
            Name = dto.Name,
            Category = dto.Category,
            Value = dto.Value,
            PurchaseDate = dto.PurchaseDate,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return new AssetReadDto
        {
            Id = asset.Id,
            SpaceId = asset.SpaceId,
            Name = asset.Name,
            Category = asset.Category,
            Value = asset.Value,
            PurchaseDate = asset.PurchaseDate,
            Description = asset.Description,
            CreatedAt = asset.CreatedAt
        };
    }

    public async Task<IEnumerable<AssetReadDto>> GetAssetsAsync()
    {
        return await _context.Assets
            .Select(asset => new AssetReadDto
            {
                Id = asset.Id,
                SpaceId = asset.SpaceId,
                Name = asset.Name,
                Category = asset.Category,
                Value = asset.Value,
                PurchaseDate = asset.PurchaseDate,
                Description = asset.Description,
                CreatedAt = asset.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId)
    {
        return await _context.Assets
            .Where(asset => asset.Space.OwnerId == userId)
            .Select(asset => new AssetReadDto
            {
                Id = asset.Id,
                SpaceId = asset.SpaceId,
                Name = asset.Name,
                Category = asset.Category,
                Value = asset.Value,
                PurchaseDate = asset.PurchaseDate,
                Description = asset.Description,
                CreatedAt = asset.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<AssetReadDto?> GetAssetByIdAsync(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
        {
            return null;
        }

        return new AssetReadDto
        {
            Id = asset.Id,
            SpaceId = asset.SpaceId,
            Name = asset.Name,
            Category = asset.Category,
            Value = asset.Value,
            PurchaseDate = asset.PurchaseDate,
            Description = asset.Description,
            CreatedAt = asset.CreatedAt
        };
    }
    public async Task<bool> DeleteAssetAsync(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
            return false;

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
        return true;
    }
}
