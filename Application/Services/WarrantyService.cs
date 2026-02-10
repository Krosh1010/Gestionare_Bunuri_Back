using Domain.Warranty;
using Application.Abstraction;
using Infrastructure.Abstraction;
using System.Threading.Tasks;

public class WarrantyService : IWarrantyService
{
    private readonly IWarrantyRepository _warrantyRepository;

    public WarrantyService(IWarrantyRepository warrantyRepository)
    {
        _warrantyRepository = warrantyRepository;
    }

    public async Task<WarrantyReadDto> CreateWarrantyAsync(WarrantyCreateDto dto)
    {
        return await _warrantyRepository.CreateWarrantyAsync(dto);
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

    public async Task<WarrantyReadDto?> PatchWarrantyByAssetIdAsync(int assetId, WarrantyUpdateDto dto)
    {
        return await _warrantyRepository.PatchWarrantyByAssetIdAsync(assetId, dto);
    }
}
