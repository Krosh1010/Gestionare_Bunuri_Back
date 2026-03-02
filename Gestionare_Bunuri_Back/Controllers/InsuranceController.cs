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
        public async Task<IActionResult> CreateInsurance([FromForm] InsuranceCreateDto dto, IFormFile? document)
        {
            var result = await _insuranceService.CreateInsuranceAsync(dto, document);
            return Ok(result);
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
        public async Task<IActionResult> PatchInsuranceByAssetId(int assetId, [FromForm] InsuranceUpdateDto dto, IFormFile? document)
        {
            var result = await _insuranceService.PatchInsuranceByAssetIdAsync(assetId, dto, document);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("by-asset/{assetId}/document/download")]
        public async Task<IActionResult> DownloadInsuranceDocument(int assetId)
        {
            var result = await _insuranceService.DownloadDocumentAsync(assetId);
            if (result == null)
                return NotFound();
            return File(result.Value.fileBytes, result.Value.contentType, result.Value.fileName);
        }

        [HttpDelete("by-asset/{assetId}/document")]
        public async Task<IActionResult> DeleteInsuranceDocument(int assetId)
        {
            var deleted = await _insuranceService.DeleteDocumentAsync(assetId);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
