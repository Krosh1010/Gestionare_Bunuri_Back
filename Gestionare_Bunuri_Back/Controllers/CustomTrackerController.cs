using Application.Abstraction;
using Domain.CustomTracker;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomTrackerController : ControllerBase
    {
        private readonly ICustomTrackerService _customTrackerService;

        public CustomTrackerController(ICustomTrackerService customTrackerService)
        {
            _customTrackerService = customTrackerService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CustomTrackerCreateDto dto)
        {
            var result = await _customTrackerService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("by-asset/{assetId}")]
        public async Task<IActionResult> GetByAssetId(int assetId)
        {
            var result = await _customTrackerService.GetByAssetIdAsync(assetId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customTrackerService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _customTrackerService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] CustomTrackerUpdateDto dto)
        {
            var result = await _customTrackerService.PatchAsync(id, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
