
using Domain.Warranty;

namespace Application.Abstraction
{
    public interface IWarrantyService
    {
        Task<WarrantyReadDto> CreateWarrantyAsync(WarrantyCreateDto dto);
        Task<WarrantyReadDto?> GetWarrantyByIdAsync(int id);
        Task<WarrantyReadDto?> GetWarrantyByAssetIdAsync(int assetId);
        Task<bool> DeleteWarrantyByAssetIdAsync(int assetId);
        Task<WarrantyReadDto?> PatchWarrantyByAssetIdAsync(int assetId, WarrantyUpdateDto dto);
    }
}
