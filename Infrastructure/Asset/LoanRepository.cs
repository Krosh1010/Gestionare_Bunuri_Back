using Domain.DbTables;
using Domain.Loan;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Asset
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;

        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoanReadDto> CreateLoanAsync(LoanCreateDto dto)
        {
            var asset = await _context.Assets.FindAsync(dto.AssetId);
            if (asset == null)
                throw new ArgumentException($"Asset with id {dto.AssetId} not found.");

            var loan = new LoanTable
            {
                AssetId = dto.AssetId,
                LoanedToName = dto.LoanedToName,
                Condition = dto.Condition,
                Notes = dto.Notes,
                LoanedAt = dto.LoanedAt
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return MapToDto(loan, asset.Name);
        }

        public async Task<LoanReadDto?> GetActiveLoanByAssetIdAsync(int assetId)
        {
            var loan = await _context.Loans
                .Include(l => l.Asset)
                .Where(l => l.AssetId == assetId && l.ReturnedAt == null)
                .OrderByDescending(l => l.LoanedAt)
                .FirstOrDefaultAsync();

            return loan == null ? null : MapToDto(loan, loan.Asset.Name);
        }

        public async Task<IEnumerable<LoanReadDto>> GetLoanHistoryByAssetIdAsync(int assetId)
        {
            var loans = await _context.Loans
                .Include(l => l.Asset)
                .Where(l => l.AssetId == assetId)
                .OrderByDescending(l => l.LoanedAt)
                .ToListAsync();

            return loans.Select(l => MapToDto(l, l.Asset.Name));
        }

        public async Task<LoanReadDto?> ReturnLoanAsync(int loanId, LoanReturnDto dto)
        {
            var loan = await _context.Loans
                .Include(l => l.Asset)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null || loan.ReturnedAt != null)
                return null;

            loan.ReturnedAt = dto.ReturnedAt;
            loan.ConditionOnReturn = dto.ConditionOnReturn;
            if (dto.Notes != null)
                loan.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return MapToDto(loan, loan.Asset.Name);
        }

        public async Task<LoanReadDto?> PatchLoanAsync(int loanId, LoanUpdateDto dto)
        {
            var loan = await _context.Loans
                .Include(l => l.Asset)
                .FirstOrDefaultAsync(l => l.Id == loanId && l.ReturnedAt == null);

            if (loan == null)
                return null;

            if (dto.LoanedToName != null)
                loan.LoanedToName = dto.LoanedToName;
            if (dto.Condition != null)
                loan.Condition = dto.Condition;
            if (dto.Notes != null)
                loan.Notes = dto.Notes;
            if (dto.LoanedAt.HasValue)
                loan.LoanedAt = dto.LoanedAt.Value;

            await _context.SaveChangesAsync();

            return MapToDto(loan, loan.Asset.Name);
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
                return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return true;
        }

        private static LoanReadDto MapToDto(LoanTable loan, string assetName) => new LoanReadDto
        {
            Id = loan.Id,
            AssetId = loan.AssetId,
            AssetName = assetName,
            LoanedToName = loan.LoanedToName,
            Condition = loan.Condition,
            Notes = loan.Notes,
            LoanedAt = loan.LoanedAt,
            ReturnedAt = loan.ReturnedAt,
            ConditionOnReturn = loan.ConditionOnReturn
        };
    }
}
