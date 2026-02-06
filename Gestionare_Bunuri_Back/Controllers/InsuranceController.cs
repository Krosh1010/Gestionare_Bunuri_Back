using Application.Abstraction;
using Domain.Insurance;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _insuranceService;

        public InsuranceController(IInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateInsurance([FromBody] InsuranceCreateDto dto)
        {
            await _insuranceService.CreateInsuranceAsync(dto);
            return NoContent();
        }

        [HttpGet("by-asset/{assetId}")]
        public async Task<IActionResult> GetInsuranceByAssetId(int assetId)
        {
            var result = await _insuranceService.GetInsuranceByAssetIdAsync(assetId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
        [HttpDelete("by-asset/{assetId}")]
        public async Task<IActionResult> DeleteInsuranceByAssetId(int assetId)
        {
            var deleted = await _insuranceService.DeleteInsuranceByAssetIdAsync(assetId);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
        [HttpPatch("by-asset/{assetId}")]
        public async Task<IActionResult> PatchInsuranceByAssetId(int assetId, [FromBody] InsuranceUpdateDto dto)
        {
            var result = await _insuranceService.PatchInsuranceByAssetIdAsync(assetId, dto);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

    }
}
