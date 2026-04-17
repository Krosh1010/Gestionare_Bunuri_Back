using Domain.DbTables;
using Domain.Document;

namespace Infrastructure.Abstraction
{
    public interface IDocumentRepository
    {
        Task<DocumentReadDto> UploadDocumentAsync(int assetId, DocumentType type, string fileName, string filePath);
        Task<DocumentReadDto?> GetDocumentAsync(int assetId, DocumentType type);
        Task<DocumentReadDto?> GetDocumentByIdAsync(int id);
        Task<bool> DeleteDocumentAsync(int id);
        Task<bool> DeleteDocumentByAssetAndTypeAsync(int assetId, DocumentType type);
        Task<DocumentReadDto> AddDocumentAsync(int assetId, DocumentType type, string fileName, string filePath, int? loanId = null);
        Task<List<DocumentReadDto>> GetDocumentsByAssetAndTypeAsync(int assetId, DocumentType type);
        Task<List<DocumentReadDto>> GetDocumentsByLoanIdAsync(int loanId);
        Task<bool> DeleteAllDocumentsByAssetAndTypeAsync(int assetId, DocumentType type);
        Task<bool> DeleteAllDocumentsByLoanIdAsync(int loanId);
    }
}
