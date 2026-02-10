using Domain.AssetDto;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Asset
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AppDbContext _context;

        public AssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto)
        {
            var asset = new Domain.DbTables.AssetTable
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

            var space = await _context.Spaces.FindAsync(asset.SpaceId);

            return new AssetReadDto
            {
                Id = asset.Id,
                SpaceId = asset.SpaceId,
                SpaceName = space?.Name ?? string.Empty,
                Name = asset.Name,
                Category = asset.Category,
                Value = asset.Value,
                PurchaseDate = asset.PurchaseDate,
                Description = asset.Description,
                CreatedAt = asset.CreatedAt
            };
        }

        public async Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId)
        {
            return await _context.Assets
                .Include(asset => asset.Space)
                .Include(asset => asset.Warranty)
                .Where(asset => asset.Space.OwnerId == userId)
                .Select(asset => new AssetReadDto
                {
                    Id = asset.Id,
                    SpaceId = asset.SpaceId,
                    SpaceName = asset.Space.Name,
                    Name = asset.Name,
                    Category = asset.Category,
                    Value = asset.Value,
                    PurchaseDate = asset.PurchaseDate,
                    Description = asset.Description,
                    CreatedAt = asset.CreatedAt,
                    WarrantyEndDate = asset.Warranty != null ? asset.Warranty.EndDate : null,
                    WarrantyStatus = asset.Warranty != null ? asset.Warranty.Status : null,
                    WarrantyProvider = asset.Warranty != null ? asset.Warranty.Provider : null,
                    WarrantyStartDate = asset.Warranty != null ? asset.Warranty.StartDate : null,
                    InsuranceEndDate = asset.Insurance != null ? asset.Insurance.EndDate : null,
                    InsuranceStatus = asset.Insurance != null ? asset.Insurance.Status : null,
                    InsuranceValue = asset.Insurance != null ? asset.Insurance.InsuredValue : null,
                    InsuranceCompany = asset.Insurance != null ? asset.Insurance.Company : null,
                    InsuranceStartDate = asset.Insurance != null ? asset.Insurance.StartDate : null
                })
                .ToListAsync();
        }

        public async Task<AssetReadDto?> GetAssetByIdAsync(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.Space)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
            {
                return null;
            }

            return new AssetReadDto
            {
                Id = asset.Id,
                SpaceId = asset.SpaceId,
                SpaceName = asset.Space.Name,
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

        public async Task<AssetReadDto?> PatchAssetAsync(int assetId, AssetUpdateDto dto)
        {
            var asset = await _context.Assets.Include(a => a.Space).FirstOrDefaultAsync(a => a.Id == assetId);
            if (asset == null)
                return null;

            if (dto.SpaceId.HasValue && dto.SpaceId.Value != asset.SpaceId)
            {
                asset.SpaceId = dto.SpaceId.Value;
                asset.Space = await _context.Spaces.FindAsync(dto.SpaceId.Value);
            }
            if (dto.Name != null)
                asset.Name = dto.Name;
            if (dto.Category != null)
                asset.Category = dto.Category;
            if (dto.Value.HasValue)
                asset.Value = dto.Value.Value;
            if (dto.PurchaseDate.HasValue)
                asset.PurchaseDate = dto.PurchaseDate.Value;
            if (dto.Description != null)
                asset.Description = dto.Description;

            await _context.SaveChangesAsync();

            var space = asset.Space ?? await _context.Spaces.FindAsync(asset.SpaceId);

            return new AssetReadDto
            {
                Id = asset.Id,
                SpaceId = asset.SpaceId,
                SpaceName = space?.Name ?? string.Empty,
                Name = asset.Name,
                Category = asset.Category,
                Value = asset.Value,
                PurchaseDate = asset.PurchaseDate,
                Description = asset.Description,
                CreatedAt = asset.CreatedAt
            };
        }
    }
}
