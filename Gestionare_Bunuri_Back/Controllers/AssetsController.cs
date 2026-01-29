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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssetReadDto>>> GetAssets()
        {
            var assets = await _assetService.GetAssetsAsync();
            return Ok(assets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssetReadDto>> GetAssetById(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null)
                return NotFound();
            return Ok(asset);
        }
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<AssetReadDto>>> GetMyAssets()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var assets = await _assetService.GetAssetsByUserIdAsync(userId);
            return Ok(assets);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var deleted = await _assetService.DeleteAssetAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
