using Application.Abstraction;
using Domain.DbTables;
using Domain.Loan;
using Infrastructure.Abstraction;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _repository;
        private readonly IDocumentService _documentService;

        public LoanService(ILoanRepository repository, IDocumentService documentService)
        {
            _repository = repository;
            _documentService = documentService;
        }

        public async Task<LoanReadDto> CreateLoanAsync(LoanCreateDto dto, List<IFormFile>? documents = null)
        {
            var result = await _repository.CreateLoanAsync(dto);

            if (documents != null && documents.Count > 0)
            {
                foreach (var file in documents)
                {
                    if (file.Length > 0)
                    {
                        var doc = await _documentService.AddDocumentAsync(dto.AssetId, DocumentType.LOAN, file, result.Id);
                        result.Documents.Add(new LoanDocumentDto { Id = doc.Id, FileName = doc.FileName });
                    }
                }
            }

            return result;
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

        public async Task<LoanReadDto?> PatchLoanAsync(int loanId, LoanUpdateDto dto, List<IFormFile>? documents = null)
        {
            var result = await _repository.PatchLoanAsync(loanId, dto);

            if (result != null && documents != null && documents.Count > 0)
            {
                foreach (var file in documents)
                {
                    if (file.Length > 0)
                    {
                        var doc = await _documentService.AddDocumentAsync(result.AssetId, DocumentType.LOAN, file, loanId);
                        result.Documents.Add(new LoanDocumentDto { Id = doc.Id, FileName = doc.FileName });
                    }
                }
            }

            return result;
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            return await _repository.DeleteLoanAsync(loanId);
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int documentId)
        {
            return await _documentService.DownloadDocumentAsync(documentId);
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            return await _documentService.DeleteDocumentAsync(documentId);
        }

        public async Task<bool> DeleteAllDocumentsAsync(int loanId)
        {
            return await _documentService.DeleteAllDocumentsByLoanIdAsync(loanId);
        }
    }
}
