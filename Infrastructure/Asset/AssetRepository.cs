
using Domain.AssetDto;
using Domain.DbTables;
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

        public async Task<PagedResult<AssetListDto>> GetAssetsByUserIdPagedAsync(int userId, AssetPagedRequest request)
        {

            var query = _context.Assets
                .Include(asset => asset.Space)
                .Include(asset => asset.Warranty)
                .Include(asset => asset.Insurance)
                .Where(asset => asset.Space.OwnerId == userId);

            // Filtrare după nume (conține, case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var nameLower = request.Name.ToLower();
                query = query.Where(asset => asset.Name.ToLower().Contains(nameLower));
            }

            // Filtrare după categorie
            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                query = query.Where(asset => asset.Category != null && asset.Category == request.Category);
            }

            // Filtrare după preț minim
            if (request.MinValue.HasValue)
            {
                query = query.Where(asset => asset.Value >= request.MinValue.Value);
            }

            // Filtrare după preț maxim
            if (request.MaxValue.HasValue)
            {
                query = query.Where(asset => asset.Value <= request.MaxValue.Value);
            }

            // Filtrare după spațiu
            if (request.SpaceId.HasValue)
            {
                query = query.Where(asset => asset.SpaceId == request.SpaceId.Value);
            }

            var totalCount = await query.CountAsync();
            var totalValue = await query.SumAsync(asset => asset.Value);

            var assets = await query
                .OrderByDescending(asset => asset.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(asset => asset.CustomTrackers)
                .ToListAsync();

            var items = assets.Select(asset =>
            {
                var latestTracker = asset.CustomTrackers?
                    .OrderByDescending(ct => ct.CreatedAt)
                    .FirstOrDefault();

                return new AssetListDto
                {
                    Id = asset.Id,
                    SpaceId = asset.SpaceId,
                    SpaceName = asset.Space.Name,
                    Name = asset.Name,
                    Category = asset.Category,
                    Value = asset.Value,
                    PurchaseDate = asset.PurchaseDate,
                    Description = asset.Description,
                    WarrantyEndDate = asset.Warranty?.EndDate,
                    WarrantyStatus = asset.Warranty?.Status,
                    InsuranceEndDate = asset.Insurance?.EndDate,
                    InsuranceStatus = asset.Insurance?.Status,
                    CustomTrackerName = latestTracker?.Name,
                    CustomTrackerEndDate = latestTracker?.EndDate,
                    CustomTrackerStatus = latestTracker?.Status
                };
            }).ToList();

            return new PagedResult<AssetListDto>
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Items = items,
                TotalValue = totalValue
            };
        }

        public async Task<AssetReadDto?> GetAssetByIdAsync(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.Space)
                .Include(a => a.Warranty)
                .Include(a => a.Insurance)
                .Include(a => a.CustomTrackers)
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null)
            {
                return null;
            }

            var latestTracker = asset.CustomTrackers?
                .OrderByDescending(ct => ct.CreatedAt)
                .FirstOrDefault();

            var warrantyDoc = asset.Documents?.FirstOrDefault(d => d.Type == DocumentType.WARRANTY);
            var insuranceDoc = asset.Documents?.FirstOrDefault(d => d.Type == DocumentType.INSURANCE);

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
                CreatedAt = asset.CreatedAt,
                WarrantyEndDate = asset.Warranty?.EndDate,
                WarrantyStatus = asset.Warranty?.Status,
                WarrantyProvider = asset.Warranty?.Provider,
                WarrantyStartDate = asset.Warranty?.StartDate,
                WarrantyDocumentId = warrantyDoc?.Id,
                WarrantyDocumentFileName = warrantyDoc?.FileName,
                InsuranceEndDate = asset.Insurance?.EndDate,
                InsuranceStatus = asset.Insurance?.Status,
                InsuranceValue = asset.Insurance?.InsuredValue,
                InsuranceCompany = asset.Insurance?.Company,
                InsuranceStartDate = asset.Insurance?.StartDate,
                InsuranceDocumentId = insuranceDoc?.Id,
                InsuranceDocumentFileName = insuranceDoc?.FileName,
                CustomTrackerName = latestTracker?.Name,
                CustomTrackerEndDate = latestTracker?.EndDate,
                CustomTrackerStatus = latestTracker?.Status
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

        public async Task<bool> PatchAssetAsync(int assetId, AssetUpdateDto dto)
        {
            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == assetId);
            if (asset == null)
                return false;

            if (dto.SpaceId.HasValue && dto.SpaceId.Value != asset.SpaceId)
                asset.SpaceId = dto.SpaceId.Value;
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
            return true;
        }
        public async Task<IEnumerable<AssetReadDto>> GetAssetsByUserIdAsync(int userId)
        {
            var assets = await _context.Assets
                .Include(asset => asset.Space)
                .Include(asset => asset.Warranty)
                .Include(asset => asset.Insurance)
                .Include(asset => asset.CustomTrackers)
                .Where(asset => asset.Space.OwnerId == userId)
                .ToListAsync();

            return assets.Select(asset =>
            {
                var latestTracker = asset.CustomTrackers?
                    .OrderByDescending(ct => ct.CreatedAt)
                    .FirstOrDefault();

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
                    CreatedAt = asset.CreatedAt,
                    WarrantyEndDate = asset.Warranty?.EndDate,
                    WarrantyStatus = asset.Warranty?.Status,
                    WarrantyProvider = asset.Warranty?.Provider,
                    WarrantyStartDate = asset.Warranty?.StartDate,
                    InsuranceEndDate = asset.Insurance?.EndDate,
                    InsuranceStatus = asset.Insurance?.Status,
                    InsuranceValue = asset.Insurance?.InsuredValue,
                    InsuranceCompany = asset.Insurance?.Company,
                    InsuranceStartDate = asset.Insurance?.StartDate,
                    CustomTrackerName = latestTracker?.Name,
                    CustomTrackerEndDate = latestTracker?.EndDate,
                    CustomTrackerStatus = latestTracker?.Status
                };
            }).ToList();
        }
    }
}
