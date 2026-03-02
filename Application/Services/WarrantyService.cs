using Domain.DbTables;
using Domain.Warranty;
using Application.Abstraction;
using Infrastructure.Abstraction;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class WarrantyService : IWarrantyService
{
    private readonly IWarrantyRepository _warrantyRepository;
    private readonly IDocumentService _documentService;

    public WarrantyService(IWarrantyRepository warrantyRepository, IDocumentService documentService)
    {
        _warrantyRepository = warrantyRepository;
        _documentService = documentService;
    }

    public async Task<WarrantyReadDto> CreateWarrantyAsync(WarrantyCreateDto dto, IFormFile? document = null)
    {
        var result = await _warrantyRepository.CreateWarrantyAsync(dto);

        if (document != null && document.Length > 0)
        {
            var doc = await _documentService.UploadDocumentAsync(dto.AssetId, DocumentType.WARRANTY, document);
            result.DocumentFileName = doc.FileName;
            result.DocumentId = doc.Id;
        }

        return result;
    }

    public async Task<WarrantyReadDto?> GetWarrantyByIdAsync(int id)
    {
        return await _warrantyRepository.GetWarrantyByIdAsync(id);
    }

    public async Task<WarrantyReadDto?> GetWarrantyByAssetIdAsync(int assetId)
    {
        return await _warrantyRepository.GetWarrantyByAssetIdAsync(assetId);
    }

    public async Task<bool> DeleteWarrantyByAssetIdAsync(int assetId)
    {
        return await _warrantyRepository.DeleteWarrantyByAssetIdAsync(assetId);
    }

    public async Task<WarrantyReadDto?> PatchWarrantyByAssetIdAsync(int assetId, WarrantyUpdateDto dto, IFormFile? document = null)
    {
        var result = await _warrantyRepository.PatchWarrantyByAssetIdAsync(assetId, dto);

        if (result != null && document != null && document.Length > 0)
        {
            var doc = await _documentService.UploadDocumentAsync(assetId, DocumentType.WARRANTY, document);
            result.DocumentFileName = doc.FileName;
            result.DocumentId = doc.Id;
        }

        return result;
    }

    public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int assetId)
    {
        return await _documentService.DownloadDocumentByAssetAsync(assetId, DocumentType.WARRANTY);
    }

    public async Task<bool> DeleteDocumentAsync(int assetId)
    {
        return await _documentService.DeleteDocumentByAssetAndTypeAsync(assetId, DocumentType.WARRANTY);
    }
}
