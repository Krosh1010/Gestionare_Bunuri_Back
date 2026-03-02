using Application.Abstraction;
using Domain.DbTables;
using Domain.Document;
using Infrastructure.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly string _uploadPath;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Documents");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public async Task<DocumentReadDto> UploadDocumentAsync(int assetId, DocumentType type, IFormFile file)
        {
            // Creăm subdirectorul pe baza tipului
            var typeFolder = type.ToString().ToLower();
            var folderPath = Path.Combine(_uploadPath, typeFolder, assetId.ToString());

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Generăm un nume unic pentru fișier
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fullPath = Path.Combine(folderPath, uniqueFileName);

            // Salvăm fișierul pe disk
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Salvăm înregistrarea în baza de date
            var result = await _documentRepository.UploadDocumentAsync(
                assetId, type, file.FileName, fullPath);

            return result;
        }

        public async Task<DocumentReadDto?> GetDocumentAsync(int assetId, DocumentType type)
        {
            return await _documentRepository.GetDocumentAsync(assetId, type);
        }

        public async Task<DocumentReadDto?> GetDocumentByIdAsync(int id)
        {
            return await _documentRepository.GetDocumentByIdAsync(id);
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int id)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(id);
            if (document == null || !File.Exists(document.FilePath))
                return null;

            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);
            var contentType = GetContentType(document.FileName);
            return (fileBytes, contentType, document.FileName);
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentByAssetAsync(int assetId, DocumentType type)
        {
            var document = await _documentRepository.GetDocumentAsync(assetId, type);
            if (document == null || !File.Exists(document.FilePath))
                return null;

            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);
            var contentType = GetContentType(document.FileName);
            return (fileBytes, contentType, document.FileName);
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            return await _documentRepository.DeleteDocumentAsync(id);
        }

        public async Task<bool> DeleteDocumentByAssetAndTypeAsync(int assetId, DocumentType type)
        {
            return await _documentRepository.DeleteDocumentByAssetAndTypeAsync(assetId, type);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
