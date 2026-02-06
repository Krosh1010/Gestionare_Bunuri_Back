using Application.Abstraction;
using Application.Abstraction.CoverageStatus;
using Domain.Warranty;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers.CoverageStatus
{
    [ApiController]
    [Route("api/coverage-status/warranty")]
    public class WarrantyStatusController : ControllerBase
    {
        private readonly IWarrantyStatusService _warrantyStatusService;

        public WarrantyStatusController(IWarrantyStatusService warrantyStatusService)
        {
            _warrantyStatusService = warrantyStatusService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetWarrantySummary()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var summary = await _warrantyStatusService.GetWarrantySummaryAsync(userId);
            return Ok(summary);
        }
        [HttpGet("expired-assets")]
        public async Task<IActionResult> GetExpiredWarrantyAssets()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var expiredAssets = await _warrantyStatusService.GetExpiredWarrantyAssetsAsync(userId);
            return Ok(expiredAssets);

        }
        [HttpGet("expiring-assets")]
        public async Task<IActionResult> GetExpiringWarrantyAssets()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var expiringAssets = await _warrantyStatusService.GetExpiringWarrantyAssetsAsync(userId);
            return Ok(expiringAssets);
        }
        [HttpGet("valid-assets")]
        public async Task<IActionResult> GetValidWarrantyAssets()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var validAssets = await _warrantyStatusService.GetValidWarrantyAssetsAsync(userId);
            return Ok(validAssets);
        }
        [HttpGet("assets-without-warranty")]
        public async Task<IActionResult> GetAssetsWithoutWarranty()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var assets = await _warrantyStatusService.GetAssetsWithoutWarrantyAsync(userId);
            return Ok(assets);
        }



    }
}
