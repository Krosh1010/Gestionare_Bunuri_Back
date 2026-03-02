
using Domain.Warranty;
using Microsoft.AspNetCore.Http;

namespace Application.Abstraction
{
    public interface IWarrantyService
    {
        Task<WarrantyReadDto> CreateWarrantyAsync(WarrantyCreateDto dto, IFormFile? document = null);
        Task<WarrantyReadDto?> GetWarrantyByIdAsync(int id);
        Task<WarrantyReadDto?> GetWarrantyByAssetIdAsync(int assetId);
        Task<bool> DeleteWarrantyByAssetIdAsync(int assetId);
        Task<WarrantyReadDto?> PatchWarrantyByAssetIdAsync(int assetId, WarrantyUpdateDto dto, IFormFile? document = null);
        Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int assetId);
        Task<bool> DeleteDocumentAsync(int assetId);
    }
}
