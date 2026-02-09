using Application.Abstraction;
using Domain.Export;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/export")]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpPost("assets-excel")]
        public async Task<IActionResult> ExportAssetsToExcel([FromBody] AssetExportRequest request)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var fileBytes = await _exportService.ExportAssetsToExcel(userId, request);

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "bunuri.xlsx");
        }

        [HttpPost("assets-pdf")]
        public async Task<IActionResult> ExportAssetsToPdf([FromBody] AssetExportRequest request)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var fileBytes = await _exportService.ExportAssetsToPdf(userId, request);

            return File(fileBytes,
                "application/pdf",
                "bunuri.pdf");
        }
        [HttpPost("assets-csv")]
        public async Task<IActionResult> ExportAssetsToCsv([FromBody] AssetExportRequest request)
        {
            var userIdString = HttpContext.Items["UserId"] as string;
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            int userId = int.Parse(userIdString);
            var fileBytes = await _exportService.ExportAssetsToCsv(userId, request);

            return File(fileBytes,
                "text/csv",
                "bunuri.csv");
        }

    }
}
