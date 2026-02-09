using Application.Abstraction;
using Domain.AssetDto;
using Domain.Export;
using Infrastructure.Abstraction;
using QuestPDF.Infrastructure;


namespace Application.Services
{
    public class ExportService : IExportService
    {
        private readonly IAssetService _assetService;
        private readonly IExportRepository _exportRepository;

        public ExportService(IAssetService assetService, IExportRepository exportRepository)
        {
            _assetService = assetService;
            _exportRepository = exportRepository;
        }

        public async Task<byte[]> ExportAssetsToExcel(int userId, AssetExportRequest request)
        {
            var filteredAssets = await GetFilteredAssets(userId, request);
            return _exportRepository.GenerateExcel(filteredAssets, request.Columns);
        }

        public async Task<byte[]> ExportAssetsToPdf(int userId, AssetExportRequest request)
        {
            var filteredAssets = await GetFilteredAssets(userId, request);
            return _exportRepository.GeneratePdf(filteredAssets, request.Columns);
        }
        public async Task<byte[]> ExportAssetsToCsv(int userId, AssetExportRequest request)
        {
            var filteredAssets = await GetFilteredAssets(userId, request);
            return _exportRepository.GenerateCsv(filteredAssets, request.Columns);
        }


        private async Task<IEnumerable<AssetReadDto>> GetFilteredAssets(int userId, AssetExportRequest request)
        {
            var assets = await _assetService.GetAssetsByUserIdAsync(userId);

            if (request.Categories != null && request.Categories.Any())
                assets = assets.Where(a => a.Category != null && request.Categories.Contains(a.Category));

            var now = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(request.InsuranceStatus) && request.InsuranceStatus != "toate")
            {
                if (request.InsuranceStatus == "expired" || request.InsuranceStatus == "expirate")
                    assets = assets.Where(a => a.InsuranceEndDate != null && a.InsuranceEndDate < now);
                else if (request.InsuranceStatus == "expiring-soon" || request.InsuranceStatus == "expira_curand")
                    assets = assets.Where(a => a.InsuranceEndDate != null && a.InsuranceEndDate >= now && a.InsuranceEndDate <= now.AddDays(30));
                else if (request.InsuranceStatus == "active")
                    assets = assets.Where(a => a.InsuranceEndDate != null && a.InsuranceEndDate > now.AddDays(30));
            }

            if (!string.IsNullOrEmpty(request.WarrantyStatus) && request.WarrantyStatus != "toate")
            {
                if (request.WarrantyStatus == "expired" || request.WarrantyStatus == "expirate")
                    assets = assets.Where(a => a.WarrantyEndDate != null && a.WarrantyEndDate < now);
                else if (request.WarrantyStatus == "expiring-soon" || request.WarrantyStatus == "expira_curand")
                    assets = assets.Where(a => a.WarrantyEndDate != null && a.WarrantyEndDate >= now && a.WarrantyEndDate <= now.AddDays(30));
                else if (request.WarrantyStatus == "active")
                    assets = assets.Where(a => a.WarrantyEndDate != null && a.WarrantyEndDate > now.AddDays(30));
            }

            return assets;
        }
    }
}
