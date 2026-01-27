using Domain.AssetDto;
using Domain.DbTables;
using Domain.AssetDto;
using Infrastructure.DataBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult<AssetReadDto>> CreateAsset([FromBody] AssetCreateDto dto)
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

            var result = new AssetReadDto
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

            return CreatedAtAction(nameof(GetAssetById), new { id = asset.Id }, result);
        }

        // READ ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetReadDto>>> GetAssets()
        {
            var assets = await _context.Assets
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

            return Ok(assets);
        }

        // READ BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetReadDto>> GetAssetById(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
                return NotFound();

            var dto = new AssetReadDto
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

            return Ok(dto);
        }
    }
}
