
using Application.Abstraction;
using Domain.DbTables;
using Domain.Insurance;
using Infrastructure.Abstraction;
using Microsoft.AspNetCore.Http;


namespace Application.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository _insuranceRepository;
        private readonly IDocumentService _documentService;

        public InsuranceService(IInsuranceRepository insuranceRepository, IDocumentService documentService)
        {
            _insuranceRepository = insuranceRepository;
            _documentService = documentService;
        }

        public async Task<InsuranceReadDto> CreateInsuranceAsync(InsuranceCreateDto dto, IFormFile? document = null)
        {
            var result = await _insuranceRepository.CreateInsuranceAsync(dto);

            if (document != null && document.Length > 0)
            {
                var doc = await _documentService.UploadDocumentAsync(dto.AssetId, DocumentType.INSURANCE, document);
                result.DocumentFileName = doc.FileName;
                result.DocumentId = doc.Id;
            }

            return result;
        }

        public async Task<InsuranceReadDto?> GetInsuranceByAssetIdAsync(int assetId)
        {
            return await _insuranceRepository.GetInsuranceByAssetIdAsync(assetId);
        }

        public async Task<bool> DeleteInsuranceByAssetIdAsync(int assetId)
        {
            return await _insuranceRepository.DeleteInsuranceByAssetIdAsync(assetId);
        }

        public async Task<InsuranceReadDto?> PatchInsuranceByAssetIdAsync(int assetId, InsuranceUpdateDto dto, IFormFile? document = null)
        {
            var result = await _insuranceRepository.PatchInsuranceByAssetIdAsync(assetId, dto);

            if (result != null && document != null && document.Length > 0)
            {
                var doc = await _documentService.UploadDocumentAsync(assetId, DocumentType.INSURANCE, document);
                result.DocumentFileName = doc.FileName;
                result.DocumentId = doc.Id;
            }

            return result;
        }

        public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int assetId)
        {
            return await _documentService.DownloadDocumentByAssetAsync(assetId, DocumentType.INSURANCE);
        }

        public async Task<bool> DeleteDocumentAsync(int assetId)
        {
            return await _documentService.DeleteDocumentByAssetAndTypeAsync(assetId, DocumentType.INSURANCE);
        }
    }
}
