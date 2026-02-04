using Application.Abstraction;
using Domain.DbTables;
using Domain.Warranty;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

public class WarrantyService : IWarrantyService
{
    private readonly AppDbContext _context;

    public WarrantyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WarrantyReadDto> CreateWarrantyAsync(WarrantyCreateDto dto)
    {
        var warranty = new WarrantyTable
        {
            AssetId = dto.AssetId,
            Provider = dto.Provider,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        _context.Warranties.Add(warranty);
        await _context.SaveChangesAsync();

        return new WarrantyReadDto
        {
            Id = warranty.Id,
            AssetId = warranty.AssetId,
            Provider = warranty.Provider,
            StartDate = warranty.StartDate,
            EndDate = warranty.EndDate,
            Status = warranty.Status // calculat automat
        };
    }

    public async Task<WarrantyReadDto?> GetWarrantyByIdAsync(int id)
    {
        var warranty = await _context.Warranties.FindAsync(id);
        if (warranty == null)
            return null;

        return new WarrantyReadDto
        {
            Id = warranty.Id,
            AssetId = warranty.AssetId,
            Provider = warranty.Provider,
            StartDate = warranty.StartDate,
            EndDate = warranty.EndDate,
            Status = warranty.Status
        };
    }
    public async Task<WarrantyReadDto?> GetWarrantyByAssetIdAsync(int assetId)
    {
        var warranty = await _context.Warranties
            .FirstOrDefaultAsync(w => w.AssetId == assetId);

        if (warranty == null)
            return null;

        return new WarrantyReadDto
        {
            Id = warranty.Id,
            AssetId = warranty.AssetId,
            Provider = warranty.Provider,
            StartDate = warranty.StartDate,
            EndDate = warranty.EndDate,
            Status = warranty.Status
        };
    }
    public async Task<bool> DeleteWarrantyByAssetIdAsync(int assetId)
    {
        var warranty = await _context.Warranties.FirstOrDefaultAsync(w => w.AssetId == assetId);
        if (warranty == null)
            return false;

        _context.Warranties.Remove(warranty);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<WarrantyReadDto?> PatchWarrantyByAssetIdAsync(int assetId, WarrantyUpdateDto dto)
    {
        var warranty = await _context.Warranties.FirstOrDefaultAsync(w => w.AssetId == assetId);
        if (warranty == null)
            return null;

        if (dto.Provider != null)
            warranty.Provider = dto.Provider;
        if (dto.StartDate.HasValue)
            warranty.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue)
            warranty.EndDate = dto.EndDate.Value;

        await _context.SaveChangesAsync();

        return new WarrantyReadDto
        {
            Id = warranty.Id,
            AssetId = warranty.AssetId,
            Provider = warranty.Provider,
            StartDate = warranty.StartDate,
            EndDate = warranty.EndDate,
            Status = warranty.Status
        };
    }
    public async Task<WarrantySummaryDto> GetWarrantySummaryAsync(int userId)
    {
        var now = DateTime.UtcNow;
        var oneMonthLater = now.AddMonths(1);

        // Ia toate asset-urile userului
        var assets = await _context.Assets
            .Include(a => a.Warranty)
            .Include(a => a.Space)
            .Where(a => a.Space.OwnerId == userId)
            .ToListAsync();

        // Ia doar garanțiile existente
        var warranties = assets
            .Where(a => a.Warranty != null)
            .Select(a => a.Warranty!)
            .ToList();

        int total = warranties.Count;
        int expired = warranties.Count(w => w.EndDate < now);
        int expiringSoon = warranties.Count(w => w.EndDate >= now && w.EndDate <= oneMonthLater);
        int validMoreThanMonth = warranties.Count(w => w.EndDate > oneMonthLater);
        int assetsWithoutWarranty = assets.Count(a => a.Warranty == null);

        return new WarrantySummaryDto
        {
            TotalCount = total,
            ExpiredCount = expired,
            ExpiringSoonCount = expiringSoon,
            ValidMoreThanMonthCount = validMoreThanMonth,
            AssetsWithoutWarrantyCount = assetsWithoutWarranty
        };
    }


}
