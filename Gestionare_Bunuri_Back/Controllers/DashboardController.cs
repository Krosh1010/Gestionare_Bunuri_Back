using Application.Abstraction;
using Domain.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("assets-summary")]
        public async Task<ActionResult<DashboardAssetSummaryDto>> GetAssetsSummary()
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);

            var summary = await _dashboardService.GetAssetSummaryAsync(userId);
            return Ok(summary);
        }
    }
}
