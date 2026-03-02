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
    }
}
