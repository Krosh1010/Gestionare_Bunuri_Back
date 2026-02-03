using Application.Abstraction;
using Domain.Warranty;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarrantyController : ControllerBase
    {
        private readonly IWarrantyService _warrantyService;

        public WarrantyController(IWarrantyService warrantyService)
        {
            _warrantyService = warrantyService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateWarranty([FromBody] WarrantyCreateDto dto)
        {
            var result = await _warrantyService.CreateWarrantyAsync(dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWarrantyById(int id)
        {
            var result = await _warrantyService.GetWarrantyByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpGet("by-asset/{assetId}")]
        public async Task<IActionResult> GetWarrantyByAssetId(int assetId)
        {
            var result = await _warrantyService.GetWarrantyByAssetIdAsync(assetId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpDelete("by-asset/{assetId}")]
        public async Task<IActionResult> DeleteWarrantyByAssetId(int assetId)
        {
            var deleted = await _warrantyService.DeleteWarrantyByAssetIdAsync(assetId);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpPatch("by-asset/{assetId}")]
        public async Task<IActionResult> PatchWarrantyByAssetId(int assetId, [FromBody] WarrantyUpdateDto dto)
        {
            var result = await _warrantyService.PatchWarrantyByAssetIdAsync(assetId, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
