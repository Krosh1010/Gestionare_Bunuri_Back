using Domain.AssetDto;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        [HttpPost]
        public async Task<ActionResult<AssetReadDto>> CreateAsset([FromBody] AssetCreateDto dto)
        {
            var result = await _assetService.CreateAssetAsync(dto);
            return CreatedAtAction(nameof(GetAssetById), new { id = result.Id }, result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetReadDto>> GetAssetById(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
                return NotFound();
            return Ok(asset);
        }
        [HttpGet("my/paged")]
        public async Task<ActionResult<PagedResult<AssetReadDto>>> GetMyAssetsPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var request = new AssetPagedRequest { Page = page, PageSize = pageSize };
            var pagedAssets = await _assetService.GetAssetsByUserIdPagedAsync(userId, request);

            return Ok(pagedAssets);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var deleted = await _assetService.DeleteAssetAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsset(int id, [FromBody] AssetUpdateDto dto)
        {
            var result = await _assetService.PatchAssetAsync(id, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
