using Domain.DbTables;
using Domain.Document;
using Microsoft.AspNetCore.Http;

namespace Application.Abstraction
{
    public interface IDocumentService
    {
        Task<DocumentReadDto> UploadDocumentAsync(int assetId, DocumentType type, IFormFile file);
        Task<DocumentReadDto?> GetDocumentAsync(int assetId, DocumentType type);
        Task<DocumentReadDto?> GetDocumentByIdAsync(int id);
        Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int id);
        Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentByAssetAsync(int assetId, DocumentType type);
        Task<bool> DeleteDocumentAsync(int id);
        Task<bool> DeleteDocumentByAssetAndTypeAsync(int assetId, DocumentType type);
        Task<DocumentReadDto> AddDocumentAsync(int assetId, DocumentType type, IFormFile file, int? loanId = null);
        Task<List<DocumentReadDto>> GetDocumentsByAssetAndTypeAsync(int assetId, DocumentType type);
        Task<List<DocumentReadDto>> GetDocumentsByLoanIdAsync(int loanId);
        Task<bool> DeleteAllDocumentsByAssetAndTypeAsync(int assetId, DocumentType type);
        Task<bool> DeleteAllDocumentsByLoanIdAsync(int loanId);
    }
}
