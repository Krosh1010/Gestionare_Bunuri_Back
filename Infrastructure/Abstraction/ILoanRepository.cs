using Domain.Loan;

namespace Infrastructure.Abstraction
{
    public interface ILoanRepository
    {
        Task<LoanReadDto> CreateLoanAsync(LoanCreateDto dto);
        Task<LoanReadDto?> GetActiveLoanByAssetIdAsync(int assetId);
        Task<IEnumerable<LoanReadDto>> GetLoanHistoryByAssetIdAsync(int assetId);
        Task<LoanReadDto?> ReturnLoanAsync(int loanId, LoanReturnDto dto);
        Task<LoanReadDto?> PatchLoanAsync(int loanId, LoanUpdateDto dto);
        Task<bool> DeleteLoanAsync(int loanId);
    }
}
