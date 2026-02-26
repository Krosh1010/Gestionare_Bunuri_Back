using Application.Abstraction.CoverageStatus;
using Domain.Insurance;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers.CoverageStatus
{
    [ApiController]
    [Route("api/coverage-status/insurance")]
    public class InsuranceStatusController : ControllerBase
    {
        private readonly IInsuranceStatusService _insuranceStatusService;

        public InsuranceStatusController(IInsuranceStatusService insuranceStatusService)
        {
            _insuranceStatusService = insuranceStatusService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetInsuranceSummary()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var summary = await _insuranceStatusService.GetInsuranceSummaryAsync(userId);
            return Ok(summary);
        }
        [HttpGet("expired-assets")]
        public async Task<IActionResult> GetExpiredInsuranceAssets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var expiredAssets = await _insuranceStatusService.GetExpiredInsuranceAssetsAsync(userId, page, pageSize);
            return Ok(expiredAssets);
        }
        [HttpGet("expiring-assets")]
        public async Task<IActionResult> GetExpiringInsuranceAssets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var expiringAssets = await _insuranceStatusService.GetExpiringInsuranceAssetsAsync(userId, page, pageSize);
            return Ok(expiringAssets);
        }
        [HttpGet("valid-assets")]
        public async Task<IActionResult> GetValidInsuranceAssets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var validAssets = await _insuranceStatusService.GetValidInsuranceAssetsAsync(userId, page, pageSize);
            return Ok(validAssets);
        }
        [HttpGet("assets-without-insurance")]
        public async Task<IActionResult> GetAssetsWithoutInsurance([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var assets = await _insuranceStatusService.GetAssetsWithoutInsuranceAsync(userId, page, pageSize);
            return Ok(assets);
        }




    }

}
