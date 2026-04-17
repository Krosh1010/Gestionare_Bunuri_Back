using Domain.Loan;
using Microsoft.AspNetCore.Http;

namespace Application.Abstraction
{
    public interface ILoanService
    {
        Task<LoanReadDto> CreateLoanAsync(LoanCreateDto dto, List<IFormFile>? documents = null);
        Task<LoanReadDto?> GetActiveLoanByAssetIdAsync(int assetId);
        Task<IEnumerable<LoanReadDto>> GetLoanHistoryByAssetIdAsync(int assetId);
        Task<LoanReadDto?> ReturnLoanAsync(int loanId, LoanReturnDto dto);
        Task<LoanReadDto?> PatchLoanAsync(int loanId, LoanUpdateDto dto, List<IFormFile>? documents = null);
        Task<bool> DeleteLoanAsync(int loanId);
        Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int documentId);
        Task<bool> DeleteDocumentAsync(int documentId);
        Task<bool> DeleteAllDocumentsAsync(int loanId);
    }
}
