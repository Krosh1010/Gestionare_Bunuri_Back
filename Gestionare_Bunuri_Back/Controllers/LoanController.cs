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
        public async Task<IActionResult> Create([FromBody] LoanCreateDto dto)
        {
            var result = await _loanService.CreateLoanAsync(dto);
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
        public async Task<IActionResult> Patch(int loanId, [FromBody] LoanUpdateDto dto)
        {
            var result = await _loanService.PatchLoanAsync(loanId, dto);
            if (result == null)
                return NotFound(new { message = "Împrumutul activ nu a fost găsit." });
            return Ok(result);
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
