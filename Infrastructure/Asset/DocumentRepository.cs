using Domain.DbTables;
using Domain.Document;
using Infrastructure.Abstraction;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Asset
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentReadDto> UploadDocumentAsync(int assetId, DocumentType type, string fileName, string filePath)
        {
            // Ștergem documentul vechi dacă există (un singur document per tip per asset)
            var existingDoc = await _context.Documents
                .FirstOrDefaultAsync(d => d.AssetId == assetId && d.Type == type);

            if (existingDoc != null)
            {
                // Ștergem fișierul vechi de pe disk
                if (File.Exists(existingDoc.FilePath))
                    File.Delete(existingDoc.FilePath);

                _context.Documents.Remove(existingDoc);
            }

            var document = new DocumentTable
            {
                AssetId = assetId,
                Type = type,
                FileName = fileName,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return new DocumentReadDto
            {
                Id = document.Id,
                AssetId = document.AssetId,
                Type = document.Type,
                FileName = document.FileName,
                FilePath = document.FilePath,
                UploadedAt = document.UploadedAt
            };
        }

        public async Task<DocumentReadDto?> GetDocumentAsync(int assetId, DocumentType type)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.AssetId == assetId && d.Type == type);

            if (document == null)
                return null;

            return new DocumentReadDto
            {
                Id = document.Id,
                AssetId = document.AssetId,
                Type = document.Type,
                FileName = document.FileName,
                FilePath = document.FilePath,
                UploadedAt = document.UploadedAt
            };
        }

        public async Task<DocumentReadDto?> GetDocumentByIdAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return null;

            return new DocumentReadDto
            {
                Id = document.Id,
                AssetId = document.AssetId,
                Type = document.Type,
                FileName = document.FileName,
                FilePath = document.FilePath,
                UploadedAt = document.UploadedAt
            };
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
                return false;

            // Ștergem fișierul de pe disk
            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDocumentByAssetAndTypeAsync(int assetId, DocumentType type)
        {
            var document = await _context.Documents
                .FirstOrDefaultAsync(d => d.AssetId == assetId && d.Type == type);

            if (document == null)
                return false;

            if (File.Exists(document.FilePath))
                File.Delete(document.FilePath);

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DocumentReadDto> AddDocumentAsync(int assetId, DocumentType type, string fileName, string filePath, int? loanId = null)
        {
            var document = new DocumentTable
            {
                AssetId = assetId,
                Type = type,
                FileName = fileName,
                FilePath = filePath,
                LoanId = loanId,
                UploadedAt = DateTime.UtcNow
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return new DocumentReadDto
            {
                Id = document.Id,
                AssetId = document.AssetId,
                Type = document.Type,
                FileName = document.FileName,
                FilePath = document.FilePath,
                LoanId = document.LoanId,
                UploadedAt = document.UploadedAt
            };
        }

        public async Task<List<DocumentReadDto>> GetDocumentsByAssetAndTypeAsync(int assetId, DocumentType type)
        {
            return await _context.Documents
                .Where(d => d.AssetId == assetId && d.Type == type)
                .Select(d => new DocumentReadDto
                {
                    Id = d.Id,
                    AssetId = d.AssetId,
                    Type = d.Type,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    LoanId = d.LoanId,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();
        }

        public async Task<List<DocumentReadDto>> GetDocumentsByLoanIdAsync(int loanId)
        {
            return await _context.Documents
                .Where(d => d.LoanId == loanId && d.Type == DocumentType.LOAN)
                .Select(d => new DocumentReadDto
                {
                    Id = d.Id,
                    AssetId = d.AssetId,
                    Type = d.Type,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    LoanId = d.LoanId,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteAllDocumentsByAssetAndTypeAsync(int assetId, DocumentType type)
        {
            var documents = await _context.Documents
                .Where(d => d.AssetId == assetId && d.Type == type)
                .ToListAsync();

            if (!documents.Any())
                return false;

            foreach (var document in documents)
            {
                if (File.Exists(document.FilePath))
                    File.Delete(document.FilePath);
            }

            _context.Documents.RemoveRange(documents);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAllDocumentsByLoanIdAsync(int loanId)
        {
            var documents = await _context.Documents
                .Where(d => d.LoanId == loanId && d.Type == DocumentType.LOAN)
                .ToListAsync();

            if (!documents.Any())
                return false;

            foreach (var document in documents)
            {
                if (File.Exists(document.FilePath))
                    File.Delete(document.FilePath);
            }

            _context.Documents.RemoveRange(documents);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
