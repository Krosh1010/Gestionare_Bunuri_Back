using Application.Abstraction;
using Domain.Loan;
using Infrastructure.Abstraction;

namespace Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _repository;

        public LoanService(ILoanRepository repository)
        {
            _repository = repository;
        }

        public async Task<LoanReadDto> CreateLoanAsync(LoanCreateDto dto)
        {
            return await _repository.CreateLoanAsync(dto);
        }

        public async Task<LoanReadDto?> GetActiveLoanByAssetIdAsync(int assetId)
        {
            return await _repository.GetActiveLoanByAssetIdAsync(assetId);
        }

        public async Task<IEnumerable<LoanReadDto>> GetLoanHistoryByAssetIdAsync(int assetId)
        {
            return await _repository.GetLoanHistoryByAssetIdAsync(assetId);
        }

        public async Task<LoanReadDto?> ReturnLoanAsync(int loanId, LoanReturnDto dto)
        {
            return await _repository.ReturnLoanAsync(loanId, dto);
        }

        public async Task<LoanReadDto?> PatchLoanAsync(int loanId, LoanUpdateDto dto)
        {
            return await _repository.PatchLoanAsync(loanId, dto);
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            return await _repository.DeleteLoanAsync(loanId);
        }
    }
}
