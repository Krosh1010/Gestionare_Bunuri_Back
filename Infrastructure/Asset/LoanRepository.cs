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

            if (loan == null) return null;

            var documents = await _context.Documents
                .Where(d => d.LoanId == loan.Id && d.Type == DocumentType.LOAN)
                .ToListAsync();

            var dto = MapToDto(loan, loan.Asset.Name);
            dto.Documents = documents.Select(d => new LoanDocumentDto
            {
                Id = d.Id,
                FileName = d.FileName
            }).ToList();
            return dto;
        }

        public async Task<IEnumerable<LoanReadDto>> GetLoanHistoryByAssetIdAsync(int assetId)
        {
            var loans = await _context.Loans
                .Include(l => l.Asset)
                .Where(l => l.AssetId == assetId)
                .OrderByDescending(l => l.LoanedAt)
                .ToListAsync();

            var loanIds = loans.Select(l => l.Id).ToList();
            var documents = await _context.Documents
                .Where(d => d.LoanId != null && loanIds.Contains(d.LoanId.Value) && d.Type == DocumentType.LOAN)
                .ToListAsync();

            var docsByLoan = documents.GroupBy(d => d.LoanId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(d => new LoanDocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName
                }).ToList());

            return loans.Select(l =>
            {
                var dto = MapToDto(l, l.Asset.Name);
                dto.Documents = docsByLoan.GetValueOrDefault(l.Id, new List<LoanDocumentDto>());
                return dto;
            });
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

            // Ștergem toate documentele asociate acestui împrumut
            var documents = await _context.Documents
                .Where(d => d.LoanId == loanId && d.Type == DocumentType.LOAN)
                .ToListAsync();

            foreach (var document in documents)
            {
                if (File.Exists(document.FilePath))
                    File.Delete(document.FilePath);
            }

            if (documents.Any())
                _context.Documents.RemoveRange(documents);

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
