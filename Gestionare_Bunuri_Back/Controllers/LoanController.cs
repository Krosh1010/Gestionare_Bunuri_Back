using Application.Abstraction;
using Domain.Loan;
using Microsoft.AspNetCore.Mvc;

namespace Gestionare_Bunuri_Back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] LoanCreateDto dto, List<IFormFile>? documents)
        {
            var result = await _loanService.CreateLoanAsync(dto, documents);
            return Ok(result);
        }
        [HttpGet("active/by-asset/{assetId}")]
        public async Task<IActionResult> GetActive(int assetId)
        {
            var result = await _loanService.GetActiveLoanByAssetIdAsync(assetId);
            if (result == null)
                return NotFound(new { message = "Bunul nu este împrumutat în momentul de față." });
            return Ok(result);
        }
        [HttpGet("history/by-asset/{assetId}")]
        public async Task<IActionResult> GetHistory(int assetId)
        {
            var result = await _loanService.GetLoanHistoryByAssetIdAsync(assetId);
            return Ok(result);
        }
        [HttpPatch("{loanId}/return")]
        public async Task<IActionResult> Return(int loanId, [FromBody] LoanReturnDto dto)
        {
            var result = await _loanService.ReturnLoanAsync(loanId, dto);
            if (result == null)
                return NotFound(new { message = "Împrumutul nu a fost găsit sau este deja returnat." });
            return Ok(result);
        }

        [HttpPatch("{loanId}")]
        public async Task<IActionResult> Patch(int loanId, [FromForm] LoanUpdateDto dto, List<IFormFile>? documents)
        {
            var result = await _loanService.PatchLoanAsync(loanId, dto, documents);
            if (result == null)
                return NotFound(new { message = "Împrumutul activ nu a fost găsit." });
            return Ok(result);
        }
        [HttpGet("document/{documentId}/download")]
        public async Task<IActionResult> DownloadLoanDocument(int documentId)
        {
            var result = await _loanService.DownloadDocumentAsync(documentId);
            if (result == null)
                return NotFound();
            return File(result.Value.fileBytes, result.Value.contentType, result.Value.fileName);
        }

        [HttpDelete("document/{documentId}")]
        public async Task<IActionResult> DeleteLoanDocument(int documentId)
        {
            var deleted = await _loanService.DeleteDocumentAsync(documentId);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{loanId}/documents")]
        public async Task<IActionResult> DeleteAllLoanDocuments(int loanId)
        {
            var deleted = await _loanService.DeleteAllDocumentsAsync(loanId);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{loanId}")]
        public async Task<IActionResult> Delete(int loanId)
        {
            var deleted = await _loanService.DeleteLoanAsync(loanId);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
