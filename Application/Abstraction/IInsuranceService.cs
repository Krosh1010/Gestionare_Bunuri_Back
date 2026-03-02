
using Domain.Insurance;
using Microsoft.AspNetCore.Http;


namespace Application.Abstraction
{
    public interface IInsuranceService
    {
        Task<InsuranceReadDto> CreateInsuranceAsync(InsuranceCreateDto dto, IFormFile? document = null);
        Task<InsuranceReadDto?> GetInsuranceByAssetIdAsync(int assetId);
        Task<bool> DeleteInsuranceByAssetIdAsync(int assetId);
        Task<InsuranceReadDto?> PatchInsuranceByAssetIdAsync(int assetId, InsuranceUpdateDto dto, IFormFile? document = null);
        Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int assetId);
        Task<bool> DeleteDocumentAsync(int assetId);
    }

}
